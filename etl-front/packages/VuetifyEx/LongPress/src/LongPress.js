// 创建长按事件监听器
function AddLongPressListener (el, binding) {
  let TouchTimeout = null; // 长按Timeout
  let StartTouchEvent = null; // 事件触发时自带的TouchEvent参数
  // 触摸事件开始
  el.addEventListener('touchstart', function (evt) {
    StartTouchEvent = evt;
    TouchTimeout = setTimeout(() => {
      el.__vue__.$emit('longpress', StartTouchEvent);
    }, binding.value || 700);
    return false;
  });
  // 触摸事件结束
  el.addEventListener('touchend', function () {
    if (TouchTimeout) {
      clearTimeout(TouchTimeout);
      TouchTimeout = null;
    }
    return false;
  });
}

export default AddLongPressListener;
