using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public ItemData data;

    public string GetInteractPrompt()
    {
        // data가 null이 아닐 때만 유효한 문자열을 반환
        if (data != null)
        {
            // C# 6.0 이상에서 사용 가능한 문자열 보간법 (Interpolated String)
            return $"{data.GetDisplayName()}\n{data.GetDescription()}";
        }

        // data가 null일 경우, 빈 문자열을 반환하여 오류를 방지
        return "";
    }

    public bool CheckGetable()
    {
        if (data != null)
        {
            return data.getable;
        }
        return true;
    }

    // 매개변수로 현재 상호작용 중인 게임 오브젝트를 받음
    public void OnInteract(GameObject interactor)
    {
        if (data == null) return;

        // 인벤토리 ViewModel 가져오기
        var viewModel2 = UI_Manager.Instance._viewModel2;
        if (viewModel2 != null)
        {
            // stackable이라면 maxStack 가져와서 추가
            int maxStack = data.maxStack; // ItemData에 maxStack 필드가 있다고 가정
            viewModel2.AddItem(data, 1);
        }
        // 아이템 월드에서 제거
        Destroy(gameObject);
    }


    //else if (curInteractGameObject.GetComponent<Door>() != null)
    //{
    //    // 현재 상호작용 게임 오브젝트가 Door 컴포넌트를 가지고 있다면,
    //    // Door의 SetState 메서드를 호출하여 상태를 변경
    //    curInteractGameObject.GetComponent<Door>().SetState();
    //}

}