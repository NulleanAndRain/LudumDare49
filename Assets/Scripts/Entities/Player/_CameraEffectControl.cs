using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class _CameraEffectControl : MonoBehaviour
{
    Camera mc;
    Volume mc_volume;
    CameraControl mcc;

    // volume effects;
    ChromaticAberration chromatic;
    LensDistortion distortion;
    Vignette vignette;
    Bloom bloom;
    ChannelMixer mixer;
    FilmGrain grain;

    [Header("test effect")]
    public float effect0InDur;
    public float effect0OutDur;

    public float ChromaticIncreasement;
    public float LensDistIntensity;

    [Header("Camera shaking")]
    public float radius;
    public float interval;
    public float grainIntensity;
    public float grainResponse;
    public float grainEnableTime;

    //[Header("Film Grain effect")]
    //public float grainIntensity;

    [Header("Hue Rotate")]
    [Range(0, .1f)]
    public float HueRotateSpeed;
    public float disableTime;

    void Start()
    {
        mc = Camera.main;
        mc_volume = mc.GetComponent<Volume>();
        mcc = mc.GetComponent<CameraControl>();

        var health = GetComponent<Health>();
        void shakeScreen (float force, float angle) {
            ShakeEffect(0.5f, force / 10);
            PivoEffect(0.5f, 0.3f, 0);
        }
        health.onGetDamage += shakeScreen;

        initVolumeComponents();
    }

    void initVolumeComponents () {
        mc_volume.profile.TryGet(out chromatic);
        mc_volume.profile.TryGet(out distortion);
        mc_volume.profile.TryGet(out vignette);
        mc_volume.profile.TryGet(out bloom);
        mc_volume.profile.TryGet(out mixer);
        mc_volume.profile.TryGet(out grain);

        chromatic.intensity.value = 0;
        distortion.intensity.value = 0;
    }

    float holdTime;
	private void Update () {
		if (Input.GetKeyDown(KeyCode.G)) {
            holdTime = Time.time;
		}
        if (Input.GetKeyUp(KeyCode.G)) {
            if (Time.time - holdTime < 0.1f) {
                PivoEffect(20);
            } else {
                PivoEffect((Time.time - holdTime) * 4 + effect0InDur + effect0OutDur);
            }
		}

		if (Input.GetKeyDown(KeyCode.F)) {
            ShakeEffect(5, 1, true);
		}

		if (Input.GetKeyDown(KeyCode.V)) {
            ToggleHueRotate();
        }
	}

    public void PivoEffect(float duration) {
        PivoEffect(duration, ChromaticIncreasement, LensDistIntensity);
    }
    Coroutine _pivo;
    /// <summary>
    /// Pivo effect (lens distortion + chromatic aberration)
    /// </summary>
    /// <param name="duration">duration of effect</param>
    public void PivoEffect (float duration, float chrIncr, float distIncr) {/*
        if (_pivo != null) {
            StopCoroutine(_pivo);
        } else {
            _chrIncOld = 0;
            _dstIncOld = 0;
        }
        _pivo = StartCoroutine(_pivoEffect(duration, chrIncr + _chrIncOld, distIncr + _dstIncOld));

        _chrIncOld = chrIncr;
        _dstIncOld = distIncr;*/

        _pivo = StartCoroutine(_pivoEffect(duration, chrIncr, distIncr));
    }

    private IEnumerator _pivoEffect(float duration, float chrIncr, float distIncr) {
        float t = Time.time;

        float chrOld = chromatic.intensity.value;
        float dstOld = distortion.intensity.value;
        chrIncr += chrOld;
        distIncr += dstOld;

        float _in = effect0InDur;
        float _out = effect0OutDur;
        //todo: very short effect duration handling;
        if (duration < _in + _out) {
            _in = duration * (_in / (_in + _out));
            _out = duration * (_out / (_in + _out));
        }

        while (t + _in > Time.time) {
            float dt = (Time.time - t) / _in;

            chromatic.intensity.value = Mathf.Lerp(chrOld, chrIncr, dt);
            distortion.intensity.value = Mathf.Lerp(dstOld, distIncr, dt);
            yield return new WaitForEndOfFrame();
        }
        chromatic.intensity.value = chrIncr;
        distortion.intensity.value = distIncr;

        float dur = duration - _in - _out;

        if (dur > 0) {
            yield return new WaitForSeconds(dur);
        }

        t = Time.time;
        while (t + _out > Time.time) {
            float dt = (Time.time - t) / _out;

            chromatic.intensity.value = Mathf.Lerp(chrIncr, chrOld, dt);
            distortion.intensity.value = Mathf.Lerp(distIncr, dstOld, dt);

            yield return new WaitForEndOfFrame();
        }
        chromatic.intensity.value = chrOld;
        distortion.intensity.value = dstOld;

    }

    /// <summary>
    /// Camera shaking effect
    /// </summary>
    /// <param name="duration">duration of effect</param>
    /// <param name="intensity">radius multipler</param>
    /// <param name="grainEnabled"> will film grain be enabled or not</param>
    public void ShakeEffect(float duration, float intensity = 1, bool grainEnabled = false) {
        if (intensity == 0 || duration == 0)
            return;
        if (radius == 0 || interval == 0)
            return;
        StartCoroutine(_shakeEffect(duration, intensity, grainEnabled));
    }

    private IEnumerator _shakeEffect (float duration, float intensity, bool grainEnabled) {
        float t = Time.time;
        float _t1;
        Vector2 _offsetOld = mcc.offset;
        Vector2 offsOld = _offsetOld;
        Vector2 offsNew;

        if (grainEnabled)
            StartCoroutine(enableGrain(grainEnableTime));

        while (t + duration > Time.time) {
            offsNew = Random.insideUnitCircle * radius * intensity;

            _t1 = Time.time;
            while (_t1 + interval > Time.time) {
                float dt = (Time.time - _t1) / interval;
                mcc.offset = Vector2.Lerp(offsOld, offsNew, dt);
                yield return new WaitForEndOfFrame();
            }

            offsOld = offsNew;
        }

        _t1 = Time.time;
        if (grainEnabled)
            StartCoroutine(disableGrain(grainEnableTime));
        while (_t1 + interval > Time.time) {
            float dt = (Time.time - _t1) / interval;
            mcc.offset = Vector2.Lerp(offsOld, _offsetOld, dt);
            yield return new WaitForEndOfFrame();
        }
        mcc.offset = _offsetOld;
    }

    private IEnumerator enableGrain(float duration) {
        grain.active = true;
        grain.intensity.value = 0;
        grain.response.value = grainResponse;

        float t = Time.time;
        while (t + duration > Time.time) {
            float dt = (Time.time - t) / duration;
            grain.intensity.value = Mathf.Lerp(0, grainIntensity, dt);
            yield return new WaitForEndOfFrame();
        }
        grain.intensity.value = grainIntensity;
    }
    private IEnumerator disableGrain (float duration) {
        float intensityOld = grain.intensity.value;

        float t = Time.time;
        while (t + duration > Time.time) {
            float dt = (Time.time - t) / duration;
            grain.intensity.value = Mathf.Lerp(intensityOld, 0, dt);
            yield return new WaitForEndOfFrame();
        }
        grain.intensity.value = 0;
        grain.active = false;
    }

    private bool hueAnimated;
    /// <summary>
    /// Enables Hue Changing
    /// </summary>
    public void ToggleHueRotate () {
        if (!hueAnimated) {
            mixer.active = true;
            hueAnimated = true;
            StartCoroutine(hueRotate());
        } else {
            hueAnimated = false;
            StartCoroutine(disableHue());
        }
    }

    private IEnumerator hueRotate () {
        float angle = 0;
        Vector3 rgb = Vector3.zero;
        int mult = 1;
        int tranzitNum = 0;
        while (hueAnimated) {
            angle += HueRotateSpeed * mult;
            if (angle > 1) {
                angle %= 1;
                tranzitNum++;
                tranzitNum %= 3;
            }

            switch (tranzitNum) {
                case 0:
                    rgb.x = Mathf.Lerp(100, 0, angle);
                    rgb.y = Mathf.Lerp(0, 100, angle);
                    rgb.z = 0;
                    mixer.redOutRedIn.value = rgb.x;
                    mixer.redOutGreenIn.value = rgb.y;
                    mixer.redOutBlueIn.value = rgb.z;

                    rgb.y = Mathf.Lerp(100, 0, angle);
                    rgb.z = Mathf.Lerp(0, 100, angle);
                    rgb.x = 0;
                    mixer.greenOutRedIn.value = rgb.x;
                    mixer.greenOutGreenIn.value = rgb.y;
                    mixer.greenOutBlueIn.value = rgb.z;

                    rgb.z = Mathf.Lerp(100, 0, angle);
                    rgb.x = Mathf.Lerp(0, 100, angle);
                    rgb.y = 0;
                    mixer.blueOutRedIn.value = rgb.x;
                    mixer.blueOutGreenIn.value = rgb.y;
                    mixer.blueOutBlueIn.value = rgb.z;
                    break;
                case 1:
                    rgb.y = Mathf.Lerp(100, 0, angle);
                    rgb.z = Mathf.Lerp(0, 100, angle);
                    rgb.x = 0;
                    mixer.redOutRedIn.value = rgb.x;
                    mixer.redOutGreenIn.value = rgb.y;
                    mixer.redOutBlueIn.value = rgb.z;

                    rgb.z = Mathf.Lerp(100, 0, angle);
                    rgb.x = Mathf.Lerp(0, 100, angle);
                    rgb.y = 0;
                    mixer.greenOutRedIn.value = rgb.x;
                    mixer.greenOutGreenIn.value = rgb.y;
                    mixer.greenOutBlueIn.value = rgb.z;

                    rgb.x = Mathf.Lerp(100, 0, angle);
                    rgb.y = Mathf.Lerp(0, 100, angle);
                    rgb.z = 0;
                    mixer.blueOutRedIn.value = rgb.x;
                    mixer.blueOutGreenIn.value = rgb.y;
                    mixer.blueOutBlueIn.value = rgb.z;
                    break;
                case 2:
                    rgb.z = Mathf.Lerp(100, 0, angle);
                    rgb.x = Mathf.Lerp(0, 100, angle);
                    rgb.y = 0;
                    mixer.redOutRedIn.value = rgb.x;
                    mixer.redOutGreenIn.value = rgb.y;
                    mixer.redOutBlueIn.value = rgb.z;

                    rgb.x = Mathf.Lerp(100, 0, angle);
                    rgb.y = Mathf.Lerp(0, 100, angle);
                    rgb.z = 0;
                    mixer.greenOutRedIn.value = rgb.x;
                    mixer.greenOutGreenIn.value = rgb.y;
                    mixer.greenOutBlueIn.value = rgb.z;

                    rgb.y = Mathf.Lerp(100, 0, angle);
                    rgb.z = Mathf.Lerp(0, 100, angle);
                    rgb.x = 0;
                    mixer.blueOutRedIn.value = rgb.x;
                    mixer.blueOutGreenIn.value = rgb.y;
                    mixer.blueOutBlueIn.value = rgb.z;
                    break;
            }
            //hueAnimated = false;
            yield return new WaitForEndOfFrame();
		}
	}

    private IEnumerator disableHue () {
        float t = Time.time;
        Vector3 rgb_r;
        rgb_r.x = mixer.redOutRedIn.value;
        rgb_r.y = mixer.redOutGreenIn.value;
        rgb_r.z = mixer.redOutBlueIn.value;

        Vector3 rgb_g;
        rgb_g.x = mixer.greenOutRedIn.value;
        rgb_g.y = mixer.greenOutGreenIn.value;
        rgb_g.z = mixer.greenOutBlueIn.value;

        Vector3 rgb_b;
        rgb_b.x = mixer.blueOutRedIn.value;
        rgb_b.y = mixer.blueOutGreenIn.value;
        rgb_b.z = mixer.blueOutBlueIn.value;

        Vector3 rgb;

        Vector3 r = new Vector3(100, 0, 0);
        Vector3 g = new Vector3(0, 100, 0);
        Vector3 b = new Vector3(0, 0, 100);
        while (t + disableTime > Time.time) {
            float dt = (Time.time - t) / disableTime;

            rgb = Vector3.Lerp(rgb_r, r, dt);
            mixer.redOutRedIn.value = rgb.x;
            mixer.redOutGreenIn.value = rgb.y;
            mixer.redOutBlueIn.value = rgb.z;

            rgb = Vector3.Lerp(rgb_g, g, dt);
            mixer.greenOutRedIn.value = rgb.x;
            mixer.greenOutGreenIn.value = rgb.y;
            mixer.greenOutBlueIn.value = rgb.z;

            rgb = Vector3.Lerp(rgb_b, b, dt);
            mixer.blueOutRedIn.value = rgb.x;
            mixer.blueOutGreenIn.value = rgb.y;
            mixer.blueOutBlueIn.value = rgb.z;
            yield return new WaitForEndOfFrame();
        }
        mixer.redOutRedIn.value = r.x;
        mixer.redOutGreenIn.value = r.y;
        mixer.redOutBlueIn.value = r.z;

        mixer.greenOutRedIn.value = g.x;
        mixer.greenOutGreenIn.value = g.y;
        mixer.greenOutBlueIn.value = g.z;

        mixer.blueOutRedIn.value = b.x;
        mixer.blueOutGreenIn.value = b.y;
        mixer.blueOutBlueIn.value = b.z;
        mixer.active = false;
    }
}
