using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NetWorkTools
{
    public interface PoolObjectInterface
    {
        //创建
        void Create(object param);
        //获得实际对象
        object GetInnerObject();
        //有效性
        bool IsValidate();
        //释放
        void Release();
    }

    public class ObjectPool
    {
        
    }
}
