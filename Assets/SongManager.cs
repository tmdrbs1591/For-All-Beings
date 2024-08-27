using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    [SerializeField] AudioSource song;
    [SerializeField] AudioClip outGameSong;
    [SerializeField] AudioClip inGameSong;

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
}
