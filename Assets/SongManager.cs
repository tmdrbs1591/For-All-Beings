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
            song.Stop(); // ���� ��� ���� ���� �����մϴ�.
        }

        song.clip = outGameSong; // ���ο� Ŭ�� ����
        song.Play(); // Ŭ�� ���
    }

    public void InGameSongPlay()
    {
        if (song.isPlaying)
        {
            song.Stop(); // ���� ��� ���� ���� �����մϴ�.
        }

        song.clip = inGameSong; // ���ο� Ŭ�� ����
        song.Play(); // Ŭ�� ���
    }

    public void BossSongPlay()
    {
        if (song.isPlaying)
        {
            song.Stop(); // ���� ��� ���� ���� �����մϴ�.
        }

        song.clip = BossSong; // ���ο� Ŭ�� ����
        song.Play(); // Ŭ�� ���
    }
}
