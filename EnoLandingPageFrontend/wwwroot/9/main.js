window.registerGlobalMouseEvents = (dotnetHelper) => {
  window._dotnetHelper = dotnetHelper;

  window.addEventListener('mousemove', mouseMoveHandler);
  window.addEventListener('mouseup', mouseUpHandler);

  function mouseMoveHandler(e) {
    window._dotnetHelper.invokeMethodAsync('OnGlobalMouseMove', e.clientX, e.clientY);
  }

  function mouseUpHandler(e) {
    window._dotnetHelper.invokeMethodAsync('OnGlobalMouseUp');
  }
};

window.unregisterGlobalMouseEvents = () => {
  window.removeEventListener('mousemove', mouseMoveHandler);
  window.removeEventListener('mouseup', mouseUpHandler);
};

window.getBoundingClientRect = (element) => {
  if (!element) {
    return null;
  }

  const rect = element.getBoundingClientRect();
  return {
    left: rect.left,
    top: rect.top,
    width: rect.width,
    height: rect.height,
    right: rect.right,
    bottom: rect.bottom,
    x: rect.x,
    y: rect.y
  };
};
