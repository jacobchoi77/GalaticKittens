using Unity.Netcode;
using UnityEngine;
using System;

// TODO: Internal dictionary for better performance on the lookup
// No longer in use, because now we use the audio manager for sfx that plays for a moment

[Serializable]
internal struct PlayerAudioClip{
    public PlayerShipAudio.PlayerClip playerClip;
    public AudioClip clip;
}


[RequireComponent(typeof(AudioSource))]
public class PlayerShipAudio : NetworkBehaviour{
    public enum PlayerClip{
        hit,
        shield,
        shoot,
    }

    [SerializeField]
    private PlayerAudioClip[] _playerClips;
    private AudioSource _source;

    private void Start(){
        _source = GetComponent<AudioSource>();
    }

    public void Play(PlayerClip playerClip){
        var clip = GetAudioClip(playerClip);
        _source.PlayOneShot(clip);
    }

    private AudioClip GetAudioClip(PlayerClip playerClip){
        foreach (var clip in _playerClips){
            if (clip.playerClip == playerClip)
                return clip.clip;
        }

        return null;
    }

}