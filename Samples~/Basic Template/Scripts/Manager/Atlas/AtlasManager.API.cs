using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    /// <summary>
    /// 
    /// ① 프로젝트 세팅
    ///     - [Project Settings - Editor] 열기
    ///     - Sprite Atlas Mode을 Sprite Atlas V2 - Enable로 설정
    ///
    /// ② 아틀라스 생성
    ///     - Assets 폴더 내 AtlasDataBase(SO)에 아틀라스 참조자를 추가하고 사용되는 씬 등록
    ///     - AtlasType 열거형에 아틀라스 타입 추가
    /// 
    /// </summary>
    
    // [중요!] 아틀라스 타입을 추가하고 싶은 경우 아래 열거형에 추가
    // Sprite 이름은 {AtlasType}_{id} 형식으로 지정
    public enum AtlasType
    {
        None,
        Jems,
    }
    
    public partial class AtlasManager
    {
        // * 새로운 아틀라스를 추가하고 싶은 경우 아래 메서드처럼 구현
        public Sprite GetJemSprite(int id) => GetSprite(AtlasType.Jems, id);
    }
}