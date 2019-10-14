using System;

namespace Utils
{
    /// <summary>
    /// 为所有单元测试函数打上标记. <br/>
    /// 打上标记的函数必须是 (1) 无参数 (2) 静态方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class UnitTest : Attribute { }
}