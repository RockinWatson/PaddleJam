using UnityEngine;
using System;
using Opertoon.Panoply;
using System.Collections.Generic;
using UnityEngine.UI;

/**
 * The PanoplyRenderer class executes global rendering tasks.
 * Copyright © Erik Loyer
 * erik@opertoon.com
 * Part of the Panoply engine
 */

namespace Opertoon.Panoply {
	
	public class PanelSubRect: System.Object {
		
		public Panel panel;
		public Rect rect;
		public Color color;
		public bool isMatte; // mattes don't get drawn for cameras that are depth only
		public bool isBorder; // borders get drawn no matter what
		
		public PanelSubRect(Panel p, Rect r, Color c, bool m) {
			panel = p;
			rect = r;
			color = c;
			isMatte = m;
			isBorder = false;
		}
			
		public PanelSubRect(Panel p, Rect r, Color c, bool m, bool b) {
			panel = p;
			rect = r;
			color = c;
			isMatte = m;
			isBorder = b;
		}

	}
	
	[ExecuteInEditMode()]
	public class PanoplyRenderer: MonoBehaviour {
		
		public Vector2 referenceScreenSize = new Vector2( 1024.0f, 768.0f );
		
		[Range(0.0f,1.0f)]
		public float matchWidthHeight = 0.5f;
		
		public bool enforceAspectRatio = false;

		public RawImage panelImagePrefab;
		
		[HideInInspector]
		public float aspectRatio;
		
		[HideInInspector]
		public Rect screenRect = new Rect();			// size of the addressable screen area after aspect ratio enforcement (if any)
		
		[HideInInspector]
		public Rect scaledScreenRect = new Rect();		// the screenRect, scaled according to the current resolutionScale
		
		Texture2D matteTexture;
		Panel[] panels;
		Caption[] captions;
		//ScriptablePanel[] scriptablePanels;
		
		public void Start() {
			
			matteTexture = new Texture2D (1, 1);
			matteTexture.SetPixel(0, 0, Color.white);
			matteTexture.Apply();
			
			UpdateInventory();

			CalculateScreenRect();
			
		}
		
		public void UpdateInventory() {
			panels = FindObjectsOfType( typeof( Panel ) ) as Panel[];
			captions = FindObjectsOfType( typeof( Caption ) ) as Caption[];
			//scriptablePanels = FindObjectsOfType( typeof( ScriptablePanel ) ) as ScriptablePanel[];
		}
		
		public void RenderBlackout( Rect drawRect ) {
			GUI.color = Color.black;
			GUI.DrawTexture( new Rect( drawRect ), matteTexture );
		}
		
		public void RenderFrame( FrameState stateA,
		                        FrameState stateB,
		                        Rect drawRect,
		                        float progress ) {
			
			Color color = Color.clear;
			
			if (( stateA != null ) && ( stateB != null )) {
				color = Color.Lerp( stateA.matteColor, stateB.matteColor, progress );
				if ( color != Color.clear ) {
					RenderFrame(drawRect, color);
				}
			}
			
		}

		public void RenderFrame(Rect drawRect, Color color) {
			if (color.a > 0) {
				GUI.color = color;
				drawRect.y = Screen.height - drawRect.y - drawRect.height;
				GUI.DrawTexture( new Rect( drawRect ), matteTexture );
			}
		}
		
		public void RenderSelection( Rect drawRect ) {
			
			//GUI.color = Color( .3, .49, .84, 1.0 );
			GUI.color = Color.gray;
			GUI.DrawTexture( new Rect( drawRect.x, drawRect.y, drawRect.width, 1.0f ), matteTexture ); 
			GUI.DrawTexture( new Rect( drawRect.x + drawRect.width - 1, drawRect.y, 1.0f, drawRect.height ), matteTexture ); 
			GUI.DrawTexture( new Rect( drawRect.x, drawRect.y + drawRect.height - 1, drawRect.width, 1.0f ), matteTexture ); 
			GUI.DrawTexture( new Rect( drawRect.x, drawRect.y, 1.0f, drawRect.height ), matteTexture ); 
			GUI.color = Color.white;
			
		}
		
		public List<PanelSubRect> GetDecomposedRectangles(List<PanelSubRect> sourceRects) {
			
			List<PanelSubRect> psRects = new List<PanelSubRect>();
			List<float> xcoords = new List<float>();
			List<float> ycoords = new List<float>();
			PanelSubRect psRectSource;
			PanelSubRect psRect;
			Camera sourceCamera;
			Rect rect;
			bool okToAdd;
			
			int i, j, k, n, o, p;
			
			// build a grid from all the unique x and y coords in the rects
			n = sourceRects.Count;
			for (i=0; i<n; i++) {
				psRect = sourceRects[i];
				if (!xcoords.Contains(psRect.rect.xMin)) xcoords.Add(psRect.rect.xMin);
				if (!xcoords.Contains(psRect.rect.xMax)) xcoords.Add(psRect.rect.xMax);
				if (!ycoords.Contains(psRect.rect.yMin)) ycoords.Add(psRect.rect.yMin);
				if (!ycoords.Contains(psRect.rect.yMax)) ycoords.Add(psRect.rect.yMax);
			}
			xcoords.Sort();
			ycoords.Sort();
			
			n = xcoords.Count;
			o = ycoords.Count;
			p = sourceRects.Count;
			for (i=0; i<(n-1); i++) {
				for (j=0; j<(o-1); j++) {
					
					// create a rect for each cell in the grid
					rect = Rect.MinMaxRect(xcoords[i], ycoords[j], xcoords[i+1], ycoords[j+1]);
					psRect = new PanelSubRect(null, rect, Color.clear, false);
					okToAdd = false;
					
					// check the rect against all of the other source rects
					for (k=0; k<p; k++) {
						psRectSource = sourceRects[k];
						if (psRectSource.rect.Contains(psRect.rect.center)) {
							if ((psRectSource.panel.frameStateA != null) && (psRectSource.panel.frameStateB != null)) {
								if (psRect.panel != null) {
									// panels with the same or greater depth override panels of lower depth
									sourceCamera = psRectSource.panel.GetComponent<Camera>();  
									if ((sourceCamera.depth >= psRect.panel.GetComponent<Camera>().depth) && (sourceCamera.clearFlags != CameraClearFlags.Depth || psRectSource.isBorder)) {										
										psRect.panel = psRectSource.panel;
										psRect.color = psRectSource.color;
										psRect.isMatte = psRectSource.isMatte;
										okToAdd = true;
									}
								} else {
									psRect.panel = psRectSource.panel;
									psRect.color = psRectSource.color;
									psRect.isMatte = psRectSource.isMatte;
									okToAdd = true;
								}
							}
						}
					}
					if (okToAdd) {
						psRects.Add(psRect);
					}
				}
			}
			
			return psRects;
			
		}

		public void CalculateScreenRect() {
			aspectRatio = referenceScreenSize.x / referenceScreenSize.y;

			if ( enforceAspectRatio ) {
				float screenAR = Screen.width / ( float )Screen.height;
				if ( aspectRatio > screenAR ) {
					screenRect.width = Screen.width;
					screenRect.height = Screen.width / aspectRatio;
				} else {
					screenRect.width = Screen.height * aspectRatio;
					screenRect.height = Screen.height;
				}
				screenRect.x = ( Screen.width - screenRect.width ) * .5f;
				screenRect.y = ( Screen.height - screenRect.height ) * .5f;
			} else {
				screenRect.x = 0.0f;
				screenRect.y = 0.0f;
				screenRect.width = Screen.width;
				screenRect.height = Screen.height;
			}
			scaledScreenRect.x = screenRect.x / PanoplyCore.resolutionScale;
			scaledScreenRect.y = screenRect.y / PanoplyCore.resolutionScale;
			scaledScreenRect.width = screenRect.width / PanoplyCore.resolutionScale;
			scaledScreenRect.height = screenRect.height / PanoplyCore.resolutionScale;
		}
		
		public void OnGUI() {
			
			GUI.depth = 10;
			
			int i = 0;
			int n = 0;
			Panel panel = null;
			Caption caption;
			//ScriptablePanel scriptablePanel = null;
			
			List<PanelSubRect> psRects = new List<PanelSubRect>();
			PanelSubRect psRect;
			Camera panelCamera;
			
			n = panels.Length;
			for( i = 0; i < n; i++ ) {
				panel = panels[ i ];
				if (panel != null) {
					panelCamera = panel.GetComponent<Camera>(); 
					if (panel.enabled && panel.gameObject.activeInHierarchy && panel.GetComponent<Camera>().enabled) {
						//RenderFrame( panel.frameStateA, panel.frameStateB, panel.frameRect, panel.progress );
						if (panelCamera.clearFlags != CameraClearFlags.Depth) {
							psRects.Add(new PanelSubRect(panel, panel.frameRect, panel.matteColor, true));
						}
						if ((panel.borderSize > 0) && (panel.borderColor.a != 0)) {
							psRects.Add(new PanelSubRect(panel, new Rect(panel.frameRect.x + panel.borderSize, panel.frameRect.y, panel.frameRect.width - panel.borderSize, panel.borderSize), panel.borderColor, false, true));
							psRects.Add(new PanelSubRect(panel, new Rect(panel.frameRect.xMax - panel.borderSize, panel.frameRect.y + panel.borderSize, panel.borderSize, panel.frameRect.height - panel.borderSize), panel.borderColor, false, true));
							psRects.Add(new PanelSubRect(panel, new Rect(panel.frameRect.x, panel.frameRect.yMax - panel.borderSize, panel.frameRect.width - panel.borderSize, panel.borderSize), panel.borderColor, false, true));
							psRects.Add(new PanelSubRect(panel, new Rect(panel.frameRect.x, panel.frameRect.y, panel.borderSize, panel.frameRect.height - panel.borderSize), panel.borderColor, false, true));
						}
					}
				}
			}

			psRects = GetDecomposedRectangles(psRects);
			n = psRects.Count;
			for (i=0; i<n; i++) {
				psRect = psRects[i];
				RenderFrame(psRect.rect, psRect.color);
			}			
			
			if (!Application.isPlaying || (Time.timeSinceLevelLoad > .1)) {
				n = captions.Length;
				for( i = 0; i < n; i++ ) {
					caption = captions[ i ];
					if (( caption != null ) && caption.enabled ) {
						caption.Render();
					}
				}
			}
			
			// draw pillarbox bars
			if ( screenRect.x > 0.0f ) {
				RenderBlackout( new Rect( 0.0f, 0.0f, screenRect.x, Screen.height ) );
				RenderBlackout( new Rect( Screen.width - screenRect.x, 0.0f, screenRect.x, Screen.height ) );
			}
			
			// draw letterbox bars
			if ( screenRect.y > 0.0f ) {
				RenderBlackout( new Rect( 0.0f, 0.0f, Screen.width, screenRect.y ) );
				RenderBlackout( new Rect( 0.0f, Screen.height - screenRect.y, Screen.width, screenRect.y ) );
			}
			
			/*n = scriptablePanels.Length;
	    	for( i = 0; i < n; i++ ) {
	    		scriptablePanel = scriptablePanels[ i ];
	    		if ( scriptablePanel != null ) {
	    			renderFrame( scriptablePanel.frameStateA, scriptablePanel.frameStateB, scriptablePanel.frameRect, scriptablePanel.progress );
	    		}
	    	}*/
			
		}
		
		public void Update() {
			CalculateScreenRect();
		}
	}
}