
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ChangeLayer : MonoBehaviour
{
    [SerializeField] private UniversalRendererData rendererData; // 拖入你的 Renderer Data
    
    //[SerializeField] private string featureName = "StencilThisWorld";
    


    //Change the layer mask of the Renderer- Render Objects
    public void ChangeRendererLayerMask(string featureName, string layerName)
    {


        ////将 Layer 名称转换为 LayerMask；也可以不做，直接下面GetMask（string）
        //LayerMask newLayerMask = LayerMask.GetMask(layerName);


        if (rendererData == null)
        {
            Debug.LogError("Renderer Data 未赋值！");
            return;
        }

        // go over all Renderer Features
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature.isActive && feature is RenderObjects renderObjectsFeature)
            {
                // 方式1：通过 Feature 名称匹配（推荐）
                if (feature.name == featureName)
                {
                    Debug.Log($"Found {featureName} !");

                    var settings = renderObjectsFeature.settings;
                    settings.filterSettings.LayerMask = LayerMask.GetMask(layerName);
                    renderObjectsFeature.settings = settings;

                    //you can put in multiple layers
                    //settings.filterSettings.LayerMask = LayerMask.GetMask("UI", "Player", "Enemy");


                    // 重要：标记为脏，让 URP 重新序列化
                    rendererData.SetDirty();
                    Debug.Log($"Render Objects [{featureName}] 的 LayerMask 已改为: {layerName}");
                    return;
                }

             
            }
        }

        Debug.LogWarning($"Can not find [{featureName}] Render Objects Feature");

    }



    //Change layer for Multiple Objects
    [SerializeField] private List<GameObject> objectsToChange = new List<GameObject>();
    public void ChangeMultipleObjectsLayer(List<GameObject> objectsToChange, LayerMask newLayerMask)
    {
        //把 LayerMask 转换成对应的图层索引（int）
        int targetLayer = Mathf.RoundToInt(Mathf.Log(newLayerMask.value, 2));

        foreach (GameObject obj in objectsToChange)
        {
            if (obj != null)
            {
                ChangeObjectLayerRecursive(obj, targetLayer);
            }
        }
    }

    // //Change layer for single object
    public void ChangeObjectLayer(GameObject obj, LayerMask newLayerMask)
    {
        //把 LayerMask 转换成对应的图层索引（int）
        int targetLayer = Mathf.RoundToInt(Mathf.Log(newLayerMask.value, 2));
        ChangeObjectLayerRecursive(obj, targetLayer);
    }


        
    private void ChangeObjectLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            ChangeObjectLayerRecursive(child.gameObject, layer);
        }
    }

}