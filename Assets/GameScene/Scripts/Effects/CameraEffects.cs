using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraEffects : MonoBehaviour
{

    public PostProcessingProfile PostProcessingProfile;
    public CameraShake CameraShake;

    private static float _zoomFactor = 0;

    public static CameraEffects Instance {get { return Camera.main.GetComponent<CameraEffects>(); }}

    private float _bloomEffectFactor = 1, _vignetteEffectFactor = 1;
    
    public void EnablePostProcessWaveEffect(bool value)
    {
        var from = (_vignetteEffectFactor + _bloomEffectFactor) * .5f;
        if (value)
        {
            iTween.ValueTo(gameObject, new Hashtable
            {
                {"name", "postProcessWave"},
                {"from", from},
                {"to", from*1.05},
                {"time", .35f},
                {"easetype",iTween.EaseType.easeOutSine},
                {"onupdatetarget", Camera.main.gameObject},
                {"onupdate", "SetPostProcessFactor"},
                {"loopType", iTween.LoopType.pingPong}
            });
        }
        else
        {
            iTween.StopByName(gameObject, "postProcessWave");
            SetPostProcessFactor(1);
        }
    }

    public void ZoomIn(float to = 0)
    {
        iTween.ValueTo(gameObject, new Hashtable
        {
            {"from", _zoomFactor},
            {"to", to},
            {"time", .5f},
            {"easetype",iTween.EaseType.easeOutSine},
            {"onupdatetarget", gameObject},
            {"onupdate", "Zoom"},
        });
    }

    public void ZoomOut(float to = 1)
    {
        iTween.Stop(gameObject);
        iTween.ValueTo(gameObject, new Hashtable
        {
            {"from", _zoomFactor},
            {"to", to},
            {"time", .7f},
            {"easetype",iTween.EaseType.easeInSine},
            {"onupdatetarget", gameObject},
            {"onupdate", "Zoom"},
        });
    }

    private void Zoom(float zoomFactor)
    {
        _zoomFactor = zoomFactor;
        Camera.main.transform.position = new Vector3(0,0,-10);//new Vector3(0, .57f*_zoomFactor, -10);
        Camera.main.orthographicSize = 7 + (7.5f - 7) * _zoomFactor;
        CameraShake.SetOriginalPos(Camera.main.transform.position);
    }

    public void SetPostProcessFactor(float postProcessFactor)
    {
        SetVignetteEffectFactor(postProcessFactor);
        SetBloomEffectFactor(postProcessFactor);
    }

    public void SetVignetteEffectFactor(float vignetteEffectFactor)
    {
        _vignetteEffectFactor = vignetteEffectFactor;
        
        VignetteModel.Settings vignetteSettings = PostProcessingProfile.vignette.settings;
        vignetteSettings.intensity = .45f * vignetteEffectFactor;
        PostProcessingProfile.vignette.settings = vignetteSettings;
    }

    public void SetBloomEffectFactor(float bloomEffectFactor)
    {
        _bloomEffectFactor = bloomEffectFactor;
        
        BloomModel.Settings bloomSettings = PostProcessingProfile.bloom.settings;
        
        bloomSettings.bloom.intensity = 1.36f*bloomEffectFactor;
        bloomSettings.bloom.threshold = .9f*1/bloomEffectFactor;
        
        PostProcessingProfile.bloom.settings = bloomSettings;
    }
    
    public void PerfectModeEffect(float effectScale)
    {
        effectScale = Mathf.Clamp(effectScale, 0, 1);

        Shake(.5f*effectScale, .4f*effectScale);
        
        var orthoSize = Camera.main.orthographicSize;
        
        iTween.ValueTo(Camera.main.gameObject, new Hashtable
        {
            {"from", orthoSize*(1-effectScale*.05f)},
            {"to", orthoSize},
            {"time", 1},
            {"onupdatetarget", Camera.main.gameObject},
            {"onupdate", "SetOrtographicSize"},
            {"easetype", iTween.EaseType.easeInSine}
        });
    }
    
    public void NormalModeEffect(float effectScale)
    {
        effectScale = Mathf.Clamp(effectScale, 0, 1);

        Shake(.2f*effectScale, .1f*effectScale);
    }

    public void Shake(float amount, float duration)
    {
        Camera.main.GetComponent<CameraShake>().shakeAmount = amount;
        Camera.main.GetComponent<CameraShake>().shakeDuration = duration;
    }

}
