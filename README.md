# DOM Util

## H函数

灵感借鉴自前端框架，

借用typescript的语法来描述

```typescript
    function H<T extends VisualElement = VisualElement> : T (
        class  ?: string | IEnumerable<string>,
        id     ?: string,
        init   ?: (T => void),
        child  ?: IEnumerable<VisualElement>
    )
```

示例：
```csharp
H(id: "container", child: new [] {
    H(new[]{"item", "flex-row"}, child: new [] {
        H("img", init: image => {
            image.style.backgroundImage = preview;
        }),
        H<Label>("img-caption", init: label => label.text = "Alt text"),
    })
});
```

## CSS解析、查询

```csharp
new UssDescriptor(".myclass").Query(context);  // 所有元素
new UssDescriptor(".myclass").Q(context);  // 单个元素
```

## 长度值简写

```csharp
setBorderRadius((10, 20, 30, 40))  // CSS TRBL简写
setBorderRadius((10, 20))  // 上下10， 左右20
setBorderRadius((10))  // 上下左右都是10
setBorderRadius((10, 20, 30))  // 上10，下30，左右都是20
```

## 用于解析自定义css变量的工具函数

```csharp
bool TryGetPixelLength(CustomStyleProperty<Length> property, out Length value);
bool TryGetPercentLength(CustomStyleProperty<Length> property, out Length value);
// 解析枚举值！
// 先定义enum MyProp { big, medium, small }
// uss中写--prop: small;
bool TryGetValue<TEnum>(CustomStyleProperty<TEnum> property, out TEnum value);
```

## 事件绑定扩展

所有函数都返回this，方便链式调用

```csharp
.On<PointerDownEvent>(ev => { /*  */ })
.Off<PointerDownEvent>(ev => { /*  */ })
.Once<MouseOverEvent>(ev => { /*  */ })

// 值变化事件
.OnChange(ev => { ev.newValue })
.OnceChange(ev => { })
.OffChange(ev => { })
```

## DOM函数扩展

```csharp
// 支持链式调用
// 所有classNames均支持参数列表、数组或IEnumerable<string>
AddElements
parent.Append(...childs)
child.AppendTo(parent)
parent.Prepend(...childs)
child.PrependTo(parent)
Closest(id, ...classNames)  // 找到最接近的父元素！
AddClass(...classNames)
RemoveClass(...classNames)

HasClass(...classNames)
```

# Rect扩展属性
```csharp
topLeft
topRight
bottomRight
bottomLeft
```
