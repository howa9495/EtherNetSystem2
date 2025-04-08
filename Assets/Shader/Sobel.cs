using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Custom/Sobel")]
public sealed class Sobel : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

    [Tooltip("Controls the colour of outline.")]
    public ColorParameter outlineColor = new ColorParameter(Color.black);//Outline顏色

    [Tooltip("Controls the thickness of outline.")]
    public FloatParameter outlineThickness = new FloatParameter(1f);

    [Tooltip("Linearly scales the depth calculation.")]
    public FloatParameter depthMultiplier = new FloatParameter(1f);

    [Tooltip("Bias (ie. power) applied to the scaled depth value.")]
    public FloatParameter depthBias = new FloatParameter(1f);

    [Tooltip("Linearly scales the depth calculation.")]
    public FloatParameter normalMultiplier = new FloatParameter(1f);

    [Tooltip("Bias (ie. power) applied to the scaled depth value.")]
    public FloatParameter normalBias = new FloatParameter(1f);


    Material m_Material;
    public bool IsActive() => m_Material != null && intensity.value > 0f;

    // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    const string kShaderName = "Post-processing/Shader/Sobel";//shader路徑

    public override void Setup()
    {
        if (Shader.Find(kShaderName) != null)
            m_Material = new Material(Shader.Find(kShaderName));//尋找相應的shader路徑
        else
            Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume Sobel is unable to load. To fix this, please edit the 'kShaderName' constant in Sobel.cs or change the name of your custom post process shader.");
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

        m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetColor("_Color", outlineColor.value);
        m_Material.SetFloat("_Thickness", outlineThickness.value);
        m_Material.SetFloat("_DepthMultiplier", depthMultiplier.value);
        m_Material.SetFloat("_DepthBias", depthBias.value);
        m_Material.SetFloat("_NormalMultiplier", normalMultiplier.value);
        m_Material.SetFloat("_NormalBias", normalBias.value);

        //HDUtils.DrawFullScreen(cmd, m_Material, destination, shaderPassId: 0);
        cmd.Blit(source, destination, m_Material, 0);//傳送主要畫面到shader的_MainTex
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
