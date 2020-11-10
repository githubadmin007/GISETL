import Message from './src/Message.vue';
import MessageHelper from './src/MessageHelper'

Message.install = function (Vue) {
  Vue.component(Message.name, Message);
}

export default Message;
export { MessageHelper };
