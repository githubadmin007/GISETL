import Vue from 'vue'
import Message from './Message.vue';

let instance;
let instances = [];
let seed = 1;

function isVNode (node) {
  return node !== null && typeof node === 'object' && Object.prototype.hasOwnProperty.call(node, 'componentOptions');
}

const MessageHelper = function (options) {
  options = options || {};
  if (typeof options === 'string') {
    options = {
      message: options
    };
  }
  let userOnClose = options.onClose;
  let id = 'message_' + seed++;

  options.onClose = function () {
    MessageHelper.close(id, userOnClose);
  };

  let MessageConstructor = Vue.extend(Message);
  instance = new MessageConstructor({
    data: options
  });
  instance.id = id;
  if (isVNode(instance.message)) {
    instance.$slots.default = [instance.message];
    instance.message = null;
  }
  instance.$mount();
  document.body.appendChild(instance.$el);
  let verticalOffset = options.offset || 20;
  instances.forEach(item => {
    verticalOffset += item.$el.offsetHeight + 16;
  });
  instance.verticalOffset = verticalOffset;
  instance.visible = true;
  instance.$el.style.zIndex = 2000; // PopupManager.nextZIndex();
  instances.push(instance);
  return instances;
}

let keys = ['success', 'warning', 'info', 'error'];
keys.forEach(type => {
  MessageHelper[type] = options => {
    if (typeof options === 'string' || typeof options === 'number') {
      options = {
        message: options
      };
    }
    options.type = type;
    return MessageHelper(options);
  };
});

MessageHelper.close = function (id, userOnClose) {
  let len = instances.length;
  let index = -1;
  let removedHeight;
  for (let i = 0; i < len; i++) {
    if (id === instances[i].id) {
      removedHeight = instances[i].$el.offsetHeight;
      index = i;
      if (typeof userOnClose === 'function') {
        userOnClose(instances[i]);
      }
      instances.splice(i, 1);
      break;
    }
  }
  if (len <= 1 || index === -1 || index > instances.length - 1) return;
  for (let i = index; i < len - 1; i++) {
    let dom = instances[i].$el;
    dom.style['top'] =
      parseInt(dom.style['top'], 10) - removedHeight - 16 + 'px';
  }
};

MessageHelper.closeAll = function () {
  for (let i = instances.length - 1; i >= 0; i--) {
    instances[i].close();
  }
};

export default MessageHelper;
