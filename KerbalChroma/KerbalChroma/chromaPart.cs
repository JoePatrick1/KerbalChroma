using UnityEngine;

namespace KerbalChroma
{
    public class chromaPart : MonoBehaviour
    {
        public Vessel parentVessel;
        public bool ischroma;
        public Renderer[] partRenderers;
        public Shader[] originalShaders;
        public Color[] originalColours;
        public bool initialised;
        public int type; //0 = vessel, 1 = kerbal


        public void Update()
        {
            if (parentVessel.loaded)
            {
                if (!initialised)
                {
                    partRenderers = GetComponentsInChildren<Renderer>();
                    int i = 0; ;
                    originalShaders = new Shader[partRenderers.Length];
                    originalColours = new Color[partRenderers.Length];
                    foreach (Renderer rend in partRenderers)
                    {
                        originalShaders[i] = rend.material.shader;
                        originalColours[i] = rend.material.color;
                        i++;
                    }
                    initialised = true;
                }

                parentVessel = this.gameObject.GetComponent<Part>().vessel;

                if ((type == 0 && chromaMain.chromaVesselActivated && chromaMain.chromaVessels.Contains(parentVessel)) || (type == 1 && chromaMain.chromaKerbalActivated && chromaMain.chromaKerbals.Contains(parentVessel)))
                {
                    foreach (Renderer rend in partRenderers)
                    {
                        if (!rend.name.Contains("Clone"))
                        {
                            rend.material.shader = Shader.Find("Unlit/Color");
                            if (!chromaMain.colourMode) rend.material.color = new Color(chromaMain.chromaRGBshared[0] / 255, chromaMain.chromaRGBshared[1] / 255, chromaMain.chromaRGBshared[2] / 255, 1);
                            else if (type == 0) rend.material.color = new Color(chromaMain.chromaRGBvessels[0] / 255, chromaMain.chromaRGBvessels[1] / 255, chromaMain.chromaRGBvessels[2] / 255, 1);
                            else if (type == 1) rend.material.color = new Color(chromaMain.chromaRGBkerbals[0] / 255, chromaMain.chromaRGBkerbals[1] / 255, chromaMain.chromaRGBkerbals[2] / 255, 1);
                        }
                    }
                }
                else
                {
                    int i = 0; ;
                    foreach (Renderer rend in partRenderers)
                    {
                        if (i < originalShaders.Length)
                        {
                            rend.material.shader = originalShaders[i];
                            rend.material.color = originalColours[i];
                            i++;
                        }
                    }
                }


            }
        }
    }
}
