using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour 
{
    // Nhập thời gian tồn tại (ví dụ 0.5 giây)
    public float lifetime = 0.5f; 

    void Start() 
    {
        // Lệnh tự xóa chính nó sau một khoảng thời gian
        Destroy(gameObject, lifetime); 
    }
}