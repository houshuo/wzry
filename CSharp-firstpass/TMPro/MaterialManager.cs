namespace TMPro
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public static class MaterialManager
    {
        private static Mask[] m_maskComponents = new Mask[0];
        private static List<MaskingMaterial> m_materialList = new List<MaskingMaterial>();

        public static void AddMaskingMaterial(Material baseMaterial, Material stencilMaterial, int stencilID)
        {
            <AddMaskingMaterial>c__AnonStorey43 storey = new <AddMaskingMaterial>c__AnonStorey43 {
                stencilMaterial = stencilMaterial
            };
            int num = m_materialList.FindIndex(new Predicate<MaskingMaterial>(storey.<>m__4F));
            if (num == -1)
            {
                MaskingMaterial item = new MaskingMaterial {
                    baseMaterial = baseMaterial,
                    stencilMaterial = storey.stencilMaterial,
                    stencilID = stencilID,
                    count = 1
                };
                m_materialList.Add(item);
            }
            else
            {
                storey.stencilMaterial = m_materialList[num].stencilMaterial;
                MaskingMaterial local1 = m_materialList[num];
                local1.count++;
            }
        }

        public static void ClearMaterials()
        {
            if (m_materialList.Count == 0)
            {
                Debug.Log("Material List has already been cleared.");
            }
            else
            {
                for (int i = 0; i < m_materialList.Count; i++)
                {
                    UnityEngine.Object.DestroyImmediate(m_materialList[i].stencilMaterial);
                    m_materialList.RemoveAt(i);
                }
            }
        }

        public static Material GetBaseMaterial(Material stencilMaterial)
        {
            <GetBaseMaterial>c__AnonStorey42 storey = new <GetBaseMaterial>c__AnonStorey42 {
                stencilMaterial = stencilMaterial
            };
            int num = m_materialList.FindIndex(new Predicate<MaskingMaterial>(storey.<>m__4E));
            if (num == -1)
            {
                return null;
            }
            return m_materialList[num].baseMaterial;
        }

        public static int GetStencilID(GameObject obj)
        {
            int num = 0;
            m_maskComponents = obj.GetComponentsInParent<Mask>();
            for (int i = 0; i < m_maskComponents.Length; i++)
            {
                if (m_maskComponents[i].MaskEnabled())
                {
                    num++;
                }
            }
            switch (num)
            {
                case 0:
                    return 0;

                case 1:
                    return 1;

                case 2:
                    return 3;

                case 3:
                    return 11;
            }
            return 0;
        }

        public static Material GetStencilMaterial(Material baseMaterial, int stencilID)
        {
            <GetStencilMaterial>c__AnonStorey41 storey = new <GetStencilMaterial>c__AnonStorey41 {
                baseMaterial = baseMaterial,
                stencilID = stencilID
            };
            if (!storey.baseMaterial.HasProperty(ShaderUtilities.ID_StencilID))
            {
                Debug.LogWarning("Selected Shader does not support Stencil Masking. Please select the Distance Field or Mobile Distance Field Shader.");
                return storey.baseMaterial;
            }
            Material stencilMaterial = null;
            int num = m_materialList.FindIndex(new Predicate<MaskingMaterial>(storey.<>m__4D));
            if (num == -1)
            {
                stencilMaterial = new Material(storey.baseMaterial) {
                    hideFlags = HideFlags.HideAndDontSave,
                    name = stencilMaterial.name + " Masking ID:" + storey.stencilID,
                    shaderKeywords = storey.baseMaterial.shaderKeywords
                };
                ShaderUtilities.GetShaderPropertyIDs();
                stencilMaterial.SetFloat(ShaderUtilities.ID_StencilID, (float) storey.stencilID);
                stencilMaterial.SetFloat(ShaderUtilities.ID_StencilComp, 3f);
                MaskingMaterial item = new MaskingMaterial {
                    baseMaterial = storey.baseMaterial,
                    stencilMaterial = stencilMaterial,
                    stencilID = storey.stencilID,
                    count = 1
                };
                m_materialList.Add(item);
            }
            else
            {
                stencilMaterial = m_materialList[num].stencilMaterial;
                MaskingMaterial local1 = m_materialList[num];
                local1.count++;
            }
            ListMaterials();
            return stencilMaterial;
        }

        public static void ListMaterials()
        {
        }

        public static void ReleaseBaseMaterial(Material baseMaterial)
        {
            <ReleaseBaseMaterial>c__AnonStorey45 storey = new <ReleaseBaseMaterial>c__AnonStorey45 {
                baseMaterial = baseMaterial
            };
            int index = m_materialList.FindIndex(new Predicate<MaskingMaterial>(storey.<>m__51));
            if (index == -1)
            {
                Debug.Log("No Masking Material exists for " + storey.baseMaterial.name);
            }
            else if (m_materialList[index].count > 1)
            {
                MaskingMaterial local1 = m_materialList[index];
                local1.count--;
                Debug.Log(string.Concat(new object[] { "Removed (1) reference to ", m_materialList[index].stencilMaterial.name, ". There are ", m_materialList[index].count, " references left." }));
            }
            else
            {
                Debug.Log(string.Concat(new object[] { "Removed last reference to ", m_materialList[index].stencilMaterial.name, " with ID ", m_materialList[index].stencilMaterial.GetInstanceID() }));
                UnityEngine.Object.DestroyImmediate(m_materialList[index].stencilMaterial);
                m_materialList.RemoveAt(index);
            }
            ListMaterials();
        }

        public static void ReleaseStencilMaterial(Material stencilMaterial)
        {
            <ReleaseStencilMaterial>c__AnonStorey46 storey = new <ReleaseStencilMaterial>c__AnonStorey46 {
                stencilMaterial = stencilMaterial
            };
            int index = m_materialList.FindIndex(new Predicate<MaskingMaterial>(storey.<>m__52));
            if (index != -1)
            {
                if (m_materialList[index].count > 1)
                {
                    MaskingMaterial local1 = m_materialList[index];
                    local1.count--;
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(m_materialList[index].stencilMaterial);
                    m_materialList.RemoveAt(index);
                }
            }
            ListMaterials();
        }

        public static void RemoveStencilMaterial(Material stencilMaterial)
        {
            <RemoveStencilMaterial>c__AnonStorey44 storey = new <RemoveStencilMaterial>c__AnonStorey44 {
                stencilMaterial = stencilMaterial
            };
            int index = m_materialList.FindIndex(new Predicate<MaskingMaterial>(storey.<>m__50));
            if (index != -1)
            {
                m_materialList.RemoveAt(index);
            }
            ListMaterials();
        }

        public static Material SetStencil(Material material, int stencilID)
        {
            material.SetFloat(ShaderUtilities.ID_StencilID, (float) stencilID);
            if (stencilID == 0)
            {
                material.SetFloat(ShaderUtilities.ID_StencilComp, 8f);
                return material;
            }
            material.SetFloat(ShaderUtilities.ID_StencilComp, 3f);
            return material;
        }

        [CompilerGenerated]
        private sealed class <AddMaskingMaterial>c__AnonStorey43
        {
            internal Material stencilMaterial;

            internal bool <>m__4F(MaterialManager.MaskingMaterial item)
            {
                return (item.stencilMaterial == this.stencilMaterial);
            }
        }

        [CompilerGenerated]
        private sealed class <GetBaseMaterial>c__AnonStorey42
        {
            internal Material stencilMaterial;

            internal bool <>m__4E(MaterialManager.MaskingMaterial item)
            {
                return (item.stencilMaterial == this.stencilMaterial);
            }
        }

        [CompilerGenerated]
        private sealed class <GetStencilMaterial>c__AnonStorey41
        {
            internal Material baseMaterial;
            internal int stencilID;

            internal bool <>m__4D(MaterialManager.MaskingMaterial item)
            {
                return ((item.baseMaterial == this.baseMaterial) && (item.stencilID == this.stencilID));
            }
        }

        [CompilerGenerated]
        private sealed class <ReleaseBaseMaterial>c__AnonStorey45
        {
            internal Material baseMaterial;

            internal bool <>m__51(MaterialManager.MaskingMaterial item)
            {
                return (item.baseMaterial == this.baseMaterial);
            }
        }

        [CompilerGenerated]
        private sealed class <ReleaseStencilMaterial>c__AnonStorey46
        {
            internal Material stencilMaterial;

            internal bool <>m__52(MaterialManager.MaskingMaterial item)
            {
                return (item.stencilMaterial == this.stencilMaterial);
            }
        }

        [CompilerGenerated]
        private sealed class <RemoveStencilMaterial>c__AnonStorey44
        {
            internal Material stencilMaterial;

            internal bool <>m__50(MaterialManager.MaskingMaterial item)
            {
                return (item.stencilMaterial == this.stencilMaterial);
            }
        }

        private class MaskingMaterial
        {
            public Material baseMaterial;
            public int count;
            public int stencilID;
            public Material stencilMaterial;
        }
    }
}

