using UnityEngine;
using UnityEngine.Events;

public class Keypad : MonoBehaviour
{
    public string password = "1234";
    public AudioClip clickSound;
    public AudioClip openSound; // 성공음
    public AudioClip noSound;   // 실패음

    public UnityEvent OnEntryAllowed;
    public UnityEvent onCorrect;
    public UnityEvent onIncorrect;

    public GameObject Obstacle;

    AudioSource audioSource;
    string userInput = "";

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        userInput = string.Empty;
    }

    public void ButtonClicked(string number)
    {
        // 키 누를 때 클릭음
        if (audioSource && clickSound) audioSource.PlayOneShot(clickSound);

        // 초과 입력 방지
        if (userInput.Length >= password.Length) return;

        userInput += number;

        // 길이가 비밀번호 길이와 같아졌을 때만 판정한다
        if (userInput.Length == password.Length)
        {
            bool ok = string.Equals(userInput, password, System.StringComparison.Ordinal);
            if (ok)
            {
                if (audioSource && openSound) audioSource.PlayOneShot(openSound);
                onCorrect?.Invoke();
                ClearPassword();

            }
            else
            {
                if (audioSource && noSound) audioSource.PlayOneShot(noSound);
                onIncorrect?.Invoke();
            }

            userInput = string.Empty; // 다음 입력을 위해 초기화
        }
    }

    void ClearPassword()
    {
        Destroy(Obstacle);
    }
   
}
