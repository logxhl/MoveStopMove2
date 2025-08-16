using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip attackSFX;
    [SerializeField] private AudioClip deadSFX;
    [SerializeField] private AudioClip deadZombieSFX;
    [SerializeField] private AudioClip clickSFX;
    [SerializeField] private AudioClip loseSFX;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false; // ⬅️ tránh tự phát khi start game
        audioSource.loop = false;        // ⬅️ chỉ phát 1 lần cho mỗi SFX
    }

    public void PlayAttack() => PlaySFX(attackSFX);
    public void DeadSFX() => PlaySFX(deadSFX);
    public void DeadZombieSFX() => PlaySFX(deadZombieSFX);
    public void ClickSFX() => PlaySFX(clickSFX);
    public void LoseSFX() => PlaySFX(loseSFX);

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    public void StopAllSFX()
    {
        audioSource.Stop();
    }
}
