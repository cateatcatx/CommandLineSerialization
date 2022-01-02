namespace Decoherence.CommandLineSerialization
{
    public interface IArgument : ISpec
    {
        /// <summary>
        /// 优先级：所有argument会按优先级的正序排列（优先级越小越靠前）
        /// </summary>
        int Priority { get; }
    }
}