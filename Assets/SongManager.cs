using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    public static SongManager instance; 


    [SerializeField] AudioSource song;
    [SerializeField] AudioClip outGameSong;
    [SerializeField] AudioClip inGameSong;
    [SerializeField] AudioClip BossSong;

    private void Awake()
    {
        instance = this;
    }

    public void OutGameSongPlay()
    {
        if (song.isPlaying)
        {
            song.Stop(); // 현재 재생 중인 곡을 정지합니다.
        }

        song.clip = outGameSong; // 새로운 클립 설정
        song.Play(); // 클립 재생
    }

    public void InGameSongPlay()
    {
        if (song.isPlaying)
        {
            song.Stop(); // 현재 재생 중인 곡을 정지합니다.
        }

        song.clip = inGameSong; // 새로운 클립 설정
        song.Play(); // 클립 재생
    }

    public void BossSongPlay()
    {
        if (song.isPlaying)
        {
            song.Stop(); // 현재 재생 중인 곡을 정지합니다.
        }

        song.clip = BossSong; // 새로운 클립 설정
        song.Play(); // 클립 재생
    }
}
