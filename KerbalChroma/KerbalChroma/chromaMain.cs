using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace KerbalChroma
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class chromaMain : MonoBehaviour
    {
        private Camera[] cameras;
        private float screenDistance = 100;
        public static float[] chromaRGBshared;
        public static float[] chromaRGBscreen;
        public static float[] chromaRGBvessels;
        public static float[] chromaRGBkerbals;
        public float[][] chromaRGBvariables = new float[4][];
        public string[][] SChromaRGBvariables = new string[4][];

        private bool chromascreenActivated = false;
        public static bool chromaVesselActivated = false;
        public static bool chromaKerbalActivated = false;
        public static bool colourMode;
        private List<Vessel> vessels = new List<Vessel>();
        private List<Vessel> kerbals = new List<Vessel>();

        private float[] defaultFarClipPlane;
        private CameraClearFlags[] defaultCameraClearFlags;

        public void Start()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                cameras = new Camera[3];
                cameras[0] = GameObject.Find("Camera 00").GetComponent<Camera>();
                cameras[1] = GameObject.Find("Camera 01").GetComponent<Camera>();
                cameras[2] = GameObject.Find("Camera ScaledSpace").GetComponent<Camera>();
                defaultFarClipPlane = new float[2];
                defaultCameraClearFlags = new CameraClearFlags[2];
                defaultFarClipPlane[0] = cameras[0].farClipPlane;
                defaultFarClipPlane[1] = cameras[1].farClipPlane;
                defaultCameraClearFlags[0] = cameras[0].clearFlags;
                defaultCameraClearFlags[1] = cameras[1].clearFlags;

                chromaRGBshared = new float[3];
                chromaRGBshared[0] = 255;
                SChromaRGBshared = new string[3];
                SChromaRGBshared[0] = "0";
                SChromaRGBshared[1] = "255";
                SChromaRGBshared[2] = "0";

                chromaRGBscreen = new float[3];
                chromaRGBscreen[0] = 255;
                SChromaRGBscreen = new string[3];
                SChromaRGBscreen[0] = "0";
                SChromaRGBscreen[1] = "255";
                SChromaRGBscreen[2] = "0";

                chromaRGBvessels = new float[3];
                chromaRGBvessels[0] = 255;
                SChromaRGBvessels = new string[3];
                SChromaRGBvessels[0] = "0";
                SChromaRGBvessels[1] = "255";
                SChromaRGBvessels[2] = "0";

                chromaRGBkerbals = new float[3];
                chromaRGBkerbals[0] = 255;
                SChromaRGBkerbals = new string[3];
                SChromaRGBkerbals[0] = "0";
                SChromaRGBkerbals[1] = "255";
                SChromaRGBkerbals[2] = "0";

                initStyles();

                chromaRGBvariables[0] = chromaRGBshared;
                chromaRGBvariables[1] = chromaRGBscreen;
                chromaRGBvariables[2] = chromaRGBvessels;
                chromaRGBvariables[3] = chromaRGBkerbals;

                SChromaRGBvariables[0] = SChromaRGBshared;
                SChromaRGBvariables[1] = SChromaRGBscreen;
                SChromaRGBvariables[2] = SChromaRGBvessels;
                SChromaRGBvariables[3] = SChromaRGBkerbals;
            }
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if ((Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Slash)) || (Input.GetKey(KeyCode.RightAlt) && Input.GetKeyDown(KeyCode.Slash)))
                {
                    mainWindowActive = !mainWindowActive;
                }

                if (chromascreenActivated && !MapView.MapIsEnabled)
                {

                    if (screenDistance < defaultFarClipPlane[0])
                    {
                        cameras[0].farClipPlane = screenDistance;
                        cameras[1].farClipPlane = screenDistance;
                        cameras[0].clearFlags = CameraClearFlags.SolidColor;
                        cameras[1].clearFlags = CameraClearFlags.SolidColor;
                    }
                    else
                    {
                        cameras[0].farClipPlane = defaultFarClipPlane[0];
                        cameras[1].farClipPlane = screenDistance;
                        cameras[0].clearFlags = defaultCameraClearFlags[0];
                        cameras[1].clearFlags = CameraClearFlags.SolidColor;
                    }

                    cameras[2].enabled = false;

                    if (!colourMode)
                    {
                        cameras[0].backgroundColor = new Color(chromaRGBshared[0] / 255, chromaRGBshared[1] / 255, chromaRGBshared[2] / 255, 1);
                        cameras[1].backgroundColor = new Color(chromaRGBshared[0] / 255, chromaRGBshared[1] / 255, chromaRGBshared[2] / 255, 1);
                    }
                    else
                    {
                        cameras[0].backgroundColor = new Color(chromaRGBscreen[0] / 255, chromaRGBscreen[1] / 255, chromaRGBscreen[2] / 255, 1);
                        cameras[1].backgroundColor = new Color(chromaRGBscreen[0] / 255, chromaRGBscreen[1] / 255, chromaRGBscreen[2] / 255, 1);
                    }
                }
                else
                {
                    cameras[0].farClipPlane = defaultFarClipPlane[0];
                    cameras[1].farClipPlane = defaultFarClipPlane[1];
                    cameras[0].clearFlags = defaultCameraClearFlags[0];
                    cameras[1].clearFlags = defaultCameraClearFlags[1];
                    cameras[2].enabled = true;
                }



                vessels.Clear();
                kerbals.Clear();
                
                foreach (Vessel vessel in FlightGlobals.Vessels)
                {
                    if (vessel.loaded)
                    {
                        bool containsEVAKerbal = false;
                        foreach (Part part in vessel.parts)
                        {
                            if (part.GetComponent<KerbalEVA>()) containsEVAKerbal = true;

                            if (part.gameObject.GetComponent<chromaPart>() == null)
                            {
                                chromaPart newchromaPart = part.gameObject.AddComponent<chromaPart>();
                                newchromaPart.parentVessel = vessel;
                                if (!containsEVAKerbal) newchromaPart.type = 0;
                                else newchromaPart.type = 1;
                                
                            }
                        }
                        if (!containsEVAKerbal)
                        {
                            if (!vessels.Contains(vessel))
                            {
                                vessels.Add(vessel);
                            }
                        }
                        else
                        {
                            if (!kerbals.Contains(vessel))
                            {
                                kerbals.Add(vessel);
                            }
                        }
                    }
                }



                for (int k = 0; k < 4; k++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (SChromaRGBvariables[k][j] != "") chromaRGBvariables[k][j] = float.Parse(SChromaRGBvariables[k][j]);
                        else chromaRGBvariables[k][j] = 0;

                        if (chromaRGBvariables[k][j] > 255)
                        {
                            chromaRGBvariables[k][j] = 255;
                            SChromaRGBvariables[k][j] = "255";
                        }
                        if (chromaRGBvariables[k][j] < 0)
                        {
                            chromaRGBvariables[k][j] = 0;
                            SChromaRGBvariables[k][j] = "0";
                        }
                    }
                }

                if (chromascreenDistance != "") screenDistance = int.Parse(chromascreenDistance);
                else screenDistance = 0;
                if (screenDistance > 10000)
                {
                    screenDistance = 10000;
                    chromascreenDistance = "10000";
                }
            }
            else
            {
                chromascreenActivated = false;
                chromaVesselActivated = false;
                chromaKerbalActivated = false;
                cameras[0].farClipPlane = defaultFarClipPlane[0];
                cameras[1].farClipPlane = defaultFarClipPlane[1];
                cameras[0].clearFlags = defaultCameraClearFlags[0];
                cameras[1].clearFlags = defaultCameraClearFlags[1];
                cameras[2].enabled = true;
            }
        }



        private Rect mainWindowRect = new Rect(100, 150, 0, 0);
        private Rect selectorWindowRect = new Rect(360, 150, 0, 0);
        private Rect screenWindowRect = new Rect(360, 150, 0, 0);
        private GUIStyle windowStyle, labelStyle, titleLabelStyle, textFieldStyle, boxStyle, buttonStyle, scrollBarStyle, scrollBarStylethumb;
        private string chromascreenDistance = "100";
        private string[] SChromaRGBshared;
        private string[] SChromaRGBscreen;
        private string[] SChromaRGBvessels;
        private string[] SChromaRGBkerbals;
        private bool mainWindowActive = false;
        private bool screenWindowActive = false;
        private bool vesselWindowActive = false;
        private bool kerbalWindowActive = false;


        void OnGUI()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (colourMode)
                {
                    mainWindowRect.height = 157;
                    selectorWindowRect.height = 308;
                    screenWindowRect.height = 156;
                }
                else
                {
                    mainWindowRect.height = 220;
                    selectorWindowRect.height = 240;
                    screenWindowRect.height = 95;
                }


                if (mainWindowActive && !MapView.MapIsEnabled) mainWindowRect = GUILayout.Window(86, mainWindowRect, mainWindow, "Chroma Settings", windowStyle);
                if (mainWindowActive && screenWindowActive && !MapView.MapIsEnabled) screenWindowRect = GUILayout.Window(87, screenWindowRect, screenEditWindow, "Chroma Screen Settings", windowStyle);
                if (mainWindowActive && vesselWindowActive && !MapView.MapIsEnabled) selectorWindowRect = GUILayout.Window(88, selectorWindowRect, vesselEditWindow, "Chroma Vessel Settings", windowStyle);
                if (mainWindowActive && kerbalWindowActive && !MapView.MapIsEnabled) selectorWindowRect = GUILayout.Window(89, selectorWindowRect, kerbalEditWindow, "Chroma Kerbal Settings", windowStyle);
            }
        }

        void mainWindow(int windowID)
        {
            GUILayout.BeginHorizontal();
            chromascreenActivated = GUILayout.Toggle(chromascreenActivated, "Enable Chroma Screen", HighLogic.Skin.toggle);
            GUILayout.BeginArea(new Rect(185, 30, 45, 30));
            if (GUILayout.Button("Edit", buttonStyle))
            {
                screenWindowActive = !screenWindowActive;
                if (screenWindowActive)
                {
                    vesselWindowActive = false;
                    kerbalWindowActive = false;
                }
            }
            GUILayout.EndArea();
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            chromaVesselActivated = GUILayout.Toggle(chromaVesselActivated, "Enable Chroma Vessel", HighLogic.Skin.toggle);
            GUILayout.BeginArea(new Rect(185, 60, 45, 30));
            if (GUILayout.Button("Edit", buttonStyle))
            {
                vesselWindowActive = !vesselWindowActive;
                if (vesselWindowActive)
                {
                    screenWindowActive = false;
                    kerbalWindowActive = false;
                }
            }
            GUILayout.EndArea();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            chromaKerbalActivated = GUILayout.Toggle(chromaKerbalActivated, "Enable Chroma Kerbal", HighLogic.Skin.toggle);
            GUILayout.BeginArea(new Rect(185, 90, 45, 30));
            if (GUILayout.Button("Edit", buttonStyle))
            {
                kerbalWindowActive = !kerbalWindowActive;
                if (kerbalWindowActive)
                {
                    screenWindowActive = false;
                    vesselWindowActive = false;
                }
            }
            GUILayout.EndArea();
            GUILayout.EndHorizontal();

           

            if (!colourMode && GUILayout.Button("Colour: Shared", HighLogic.Skin.button)) colourMode = true;
            if (colourMode && GUILayout.Button("Colour: Individual",HighLogic.Skin.button)) colourMode = false;

            if (!colourMode)
            {
                GUILayout.BeginVertical(boxStyle);
                GUILayout.Label("Colour", titleLabelStyle);
                GUILayout.BeginHorizontal("", GUIStyle.none);
                GUILayout.Label("R:", labelStyle);
                SChromaRGBshared[0] = GUILayout.TextField(SChromaRGBshared[0], 3, textFieldStyle);
                SChromaRGBshared[0] = Regex.Replace(SChromaRGBshared[0], "[^0-9]", "");
                GUILayout.Label("G:", labelStyle);
                SChromaRGBshared[1] = GUILayout.TextField(SChromaRGBshared[1], 3, textFieldStyle);
                SChromaRGBshared[1] = Regex.Replace(SChromaRGBshared[1], "[^0-9]", "");
                GUILayout.Label("B:", labelStyle);
                SChromaRGBshared[2] = GUILayout.TextField(SChromaRGBshared[2], 3, textFieldStyle);
                SChromaRGBshared[2] = Regex.Replace(SChromaRGBshared[2], "[^0-9]", "");
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            GUI.DragWindow();
        }


        void screenEditWindow(int windowID)
        {

            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("Distance", titleLabelStyle);
            GUILayout.BeginHorizontal("", GUIStyle.none);
            chromascreenDistance = GUILayout.TextField(chromascreenDistance, 5, HighLogic.Skin.textField);
            chromascreenDistance = Regex.Replace(chromascreenDistance, "[^0-9]", "");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (colourMode)
            {
                GUILayout.BeginVertical(boxStyle);
                GUILayout.Label("Colour", titleLabelStyle);
                GUILayout.BeginHorizontal("", GUIStyle.none);
                GUILayout.Label("R:", labelStyle);
                SChromaRGBscreen[0] = GUILayout.TextField(SChromaRGBscreen[0], 3, textFieldStyle);
                SChromaRGBscreen[0] = Regex.Replace(SChromaRGBscreen[0], "[^0-9]", "");
                GUILayout.Label("G:", labelStyle);
                SChromaRGBscreen[1] = GUILayout.TextField(SChromaRGBscreen[1], 3, textFieldStyle);
                SChromaRGBscreen[1] = Regex.Replace(SChromaRGBscreen[1], "[^0-9]", "");
                GUILayout.Label("B:", labelStyle);
                SChromaRGBscreen[2] = GUILayout.TextField(SChromaRGBscreen[2], 3, textFieldStyle);
                SChromaRGBscreen[2] = Regex.Replace(SChromaRGBscreen[2], "[^0-9]", "");
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            GUI.DragWindow();
        }


        public static List<Vessel> chromaVessels = new List<Vessel>();
        public static List<Vessel> chromaKerbals = new List<Vessel>();
        bool[] togglesV = new bool[1];
        bool[] togglesK = new bool[1];
        Vector2 scrollV;
        Vector2 scrollK;

        void vesselEditWindow(int windowID)
        {
            GUI.skin = HighLogic.Skin;
            scrollV = GUILayout.BeginScrollView(scrollV, false, false);
            GUILayout.Label("Loaded Vessels (" + vessels.Count + ")", titleLabelStyle);
            GUI.skin = null;

            if (togglesV.Length < vessels.Count)
            {
                bool[] temp = new bool[togglesV.Length];
                togglesV.CopyTo(temp, 0);
                togglesV = new bool[vessels.Count];
                temp.CopyTo(togglesV, 0);
            }

            //int i = 0;
            foreach (Vessel vessel in vessels)
            {
                togglesV[vessels.IndexOf(vessel)] = GUILayout.Toggle(togglesV[vessels.IndexOf(vessel)], vessel.vesselName, HighLogic.Skin.toggle);
                if (togglesV[vessels.IndexOf(vessel)] && !chromaVessels.Contains(vessel))
                    chromaVessels.Add(vessel);

                if (!togglesV[vessels.IndexOf(vessel)] && chromaVessels.Contains(vessel))
                    chromaVessels.Remove(vessel);
               // i++;
            }
            GUILayout.EndScrollView();

            if (colourMode)
            {
                GUILayout.BeginVertical(boxStyle);
                GUILayout.Label("Colour", titleLabelStyle);
                GUILayout.BeginHorizontal("", GUIStyle.none);
                GUILayout.Label("R:", labelStyle);
                SChromaRGBvessels[0] = GUILayout.TextField(SChromaRGBvessels[0], 3, textFieldStyle);
                SChromaRGBvessels[0] = Regex.Replace(SChromaRGBvessels[0], "[^0-9]", "");
                GUILayout.Label("G:", labelStyle);
                SChromaRGBvessels[1] = GUILayout.TextField(SChromaRGBvessels[1], 3, textFieldStyle);
                SChromaRGBvessels[1] = Regex.Replace(SChromaRGBvessels[1], "[^0-9]", "");
                GUILayout.Label("B:", labelStyle);
                SChromaRGBvessels[2] = GUILayout.TextField(SChromaRGBvessels[2], 3, textFieldStyle);
                SChromaRGBvessels[2] = Regex.Replace(SChromaRGBvessels[2], "[^0-9]", "");
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            GUI.DragWindow();
        }

        void kerbalEditWindow(int windowID)
        {
            GUI.skin = HighLogic.Skin;
            scrollK = GUILayout.BeginScrollView(scrollK, false, false);
            GUILayout.Label("Loaded Kerbals (" + kerbals.Count + ")", titleLabelStyle);
            GUI.skin = null;

            if (togglesK.Length < kerbals.Count)
            {
                bool[] temp = new bool[togglesK.Length];
                togglesK.CopyTo(temp, 0);
                togglesK = new bool[kerbals.Count];
                temp.CopyTo(togglesK, 0);
            }

            int i = 0;
            foreach (Vessel kerbal in kerbals)
            {
                togglesK[kerbals.IndexOf(kerbal)] = GUILayout.Toggle(togglesK[kerbals.IndexOf(kerbal)], kerbal.vesselName, HighLogic.Skin.toggle);
                if (togglesK[kerbals.IndexOf(kerbal)] && !chromaKerbals.Contains(kerbal))
                    chromaKerbals.Add(kerbal);

                if (!togglesK[kerbals.IndexOf(kerbal)] && chromaKerbals.Contains(kerbal))
                    chromaKerbals.Remove(kerbal);
                i++;
            }
            GUILayout.EndScrollView();

            if (colourMode)
            {
                GUILayout.BeginVertical(boxStyle);
                GUILayout.Label("Colour", titleLabelStyle);
                GUILayout.BeginHorizontal("", GUIStyle.none);
                GUILayout.Label("R:", labelStyle);
                SChromaRGBkerbals[0] = GUILayout.TextField(SChromaRGBkerbals[0], 3, textFieldStyle);
                SChromaRGBkerbals[0] = Regex.Replace(SChromaRGBkerbals[0], "[^0-9]", "");
                GUILayout.Label("G:", labelStyle);
                SChromaRGBkerbals[1] = GUILayout.TextField(SChromaRGBkerbals[1], 3, textFieldStyle);
                SChromaRGBkerbals[1] = Regex.Replace(SChromaRGBkerbals[1], "[^0-9]", "");
                GUILayout.Label("B:", labelStyle);
                SChromaRGBkerbals[2] = GUILayout.TextField(SChromaRGBkerbals[2], 3, textFieldStyle);
                SChromaRGBkerbals[2] = Regex.Replace(SChromaRGBkerbals[2], "[^0-9]", "");
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            GUI.DragWindow();
        }

        void initStyles()
        {
            windowStyle = new GUIStyle(HighLogic.Skin.window);
            windowStyle.fixedWidth = 235;
            labelStyle = new GUIStyle(HighLogic.Skin.label);
            titleLabelStyle = new GUIStyle(HighLogic.Skin.label);
            titleLabelStyle.stretchWidth = true;
            titleLabelStyle.alignment = TextAnchor.MiddleCenter;
            textFieldStyle = new GUIStyle(HighLogic.Skin.textField);
            textFieldStyle.fixedWidth = 42;
            textFieldStyle.margin.bottom = 5;
            boxStyle = new GUIStyle(HighLogic.Skin.box);
            boxStyle.fixedHeight = 58;
            buttonStyle = new GUIStyle(HighLogic.Skin.button);
            buttonStyle.fixedWidth = 42;
            scrollBarStyle = new GUIStyle(HighLogic.Skin.verticalScrollbar);
            scrollBarStylethumb = new GUIStyle(HighLogic.Skin.verticalScrollbarThumb);
        }

    }
}