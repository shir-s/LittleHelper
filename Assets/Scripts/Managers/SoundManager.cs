using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Managers
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;
        public AudioSource ambienceSource;

        [Header("Ambient Sounds")]
        public AudioClip backgroundMusic;
        public AudioClip freezerAmbience;
        public AudioClip gardenAmbience;
        public AudioClip ratsAmbience;

        [Header("SFX")]
        public AudioClip buyUpgrade;
        public AudioClip collectCandy;
        public AudioClip collectItem;
        public AudioClip footstep;
        public AudioClip hit;
        public AudioClip molePop;
        public AudioClip openContainer;

        [Header("Scene Specific Music")]
        public AudioClip startAndWinMusic;

        [Range(0f, 1f)]
        public float musicVolume = 0.1f;
        public float freezerVolume = 0.5f;
        public float gardenVolume = 0.1f;
        public float ratsVolume = 0.1f;
        [Range(0f, 1f)]
        public float startAndWinVolume = 0.4f;
        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            PlayMusicByScene(SceneManager.GetActiveScene().name);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            PlayMusicByScene(scene.name);
        }

        public void PlayMusicByScene(string sceneName)
        {
            StopMusic();

            switch (sceneName)
            {
                case "Start":
                case "Win Scene":
                    PlayStartOrWinMusic();
                    break;
                case "SandBox":
                    PlayBackgroundMusic();
                    break;
            }
        }
        
        public void PlayBackgroundMusic()
        {
            if (backgroundMusic != null && musicSource.clip != backgroundMusic)
            {
                musicSource.clip = backgroundMusic;
                musicSource.loop = true;
                musicSource.volume = musicVolume;
                musicSource.Play();
            }
        }

        
        public void PlayStartOrWinMusic()
        {
            musicSource.Stop();
            musicSource.clip = startAndWinMusic;
            musicSource.volume = startAndWinVolume;
            musicSource.loop = true;
            musicSource.Play();
        }


        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip != null)
                sfxSource.PlayOneShot(clip, volume);
        }

        // SFX Helpers
        public void PlayFootstep() => PlaySFX(footstep);
        public void PlayCollectCandy(float volume = 1f) => PlaySFX(collectCandy, volume);
        public void PlayCollectItem() => PlaySFX(collectItem);
        public void PlayBuyUpgrade() => PlaySFX(buyUpgrade);
        public void PlayHit() => PlaySFX(hit, 0.3f);
        public void PlayMolePop() => PlaySFX(molePop, 0.2f);
        public void PlayOpenContainer() => PlaySFX(openContainer, 0.5f);

        // Ambience
        public void PlayFreezerAmbience()
        {
            PlayAmbience(freezerAmbience, freezerVolume);
        }

        public void PlayGardenAmbience()
        {
            PlayAmbience(gardenAmbience, gardenVolume);
        }

        public void PlayRatsAmbience()
        {
            PlayAmbience(ratsAmbience, ratsVolume);
        }

        private void PlayAmbience(AudioClip clip, float volume)
        {
            if (ambienceSource.clip == clip && ambienceSource.isPlaying) return;

            ambienceSource.Stop();
            ambienceSource.clip = clip;
            ambienceSource.volume = volume;
            ambienceSource.loop = true;
            ambienceSource.Play();
        }

        public void StopAmbience()
        {
            ambienceSource.Stop();
        }

        private void StopAmbienceIfPlaying(AudioClip clip)
        {
            if (ambienceSource.isPlaying && ambienceSource.clip == clip)
                ambienceSource.Stop();
        }

        public void StopRatsAmbience() => StopAmbienceIfPlaying(ratsAmbience);
        public void StopGardenAmbience() => StopAmbienceIfPlaying(gardenAmbience);
        public void StopFreezerAmbience() => StopAmbienceIfPlaying(freezerAmbience);
    }
}
