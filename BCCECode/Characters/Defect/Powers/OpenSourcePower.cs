using MegaCrit.Sts2.Core.Entities.Powers;

namespace BCCE.BCCECode.Characters.Defect.Powers;

// 开源-观看抽牌堆时，卡牌将按实际发牌顺序排列
public sealed class OpenSourcePower : DefectPower
{
    public override PowerType Type => PowerType.Buff;
    //定义能力类型：增益
    public override PowerStackType StackType => PowerStackType.Single;
    //定义叠加类型：单一
}
