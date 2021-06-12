/**
 * From:
 * https://github.com/ChrisCavs/t-writer.js
 */

var classCallCheck = function (instance, Constructor) {
  if (!(instance instanceof Constructor)) {
    throw new TypeError('Cannot call a class as a function');
  }
};

var createClass = (function () {
  function defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
      var descriptor = props[i];
      descriptor.enumerable = descriptor.enumerable || false;
      descriptor.configurable = true;
      if ('value' in descriptor) descriptor.writable = true;
      Object.defineProperty(target, descriptor.key, descriptor);
    }
  }

  return function (Constructor, protoProps, staticProps) {
    if (protoProps) defineProperties(Constructor.prototype, protoProps);
    if (staticProps) defineProperties(Constructor, staticProps);
    return Constructor;
  };
})();

var defaultOptions = {
  loop: false,
  animateCursor: true,

  blinkSpeed: 400,

  typeSpeed: 90,
  deleteSpeed: 40,

  typeSpeedMin: 65,
  typeSpeedMax: 115,

  deleteSpeedMin: 40,
  deleteSpeedMax: 90,

  typeClass: 'type-span',
  cursorClass: 'cursor-span',

  typeColor: 'black',
  cursorColor: 'black',
};

var Cursor = (function () {
  function Cursor(el, speed) {
    classCallCheck(this, Cursor);

    this.el = el;
    this.speed = speed;

    this.faded = false;

    this.initialAssignment();
    this.el.addEventListener('transitionend', this.logic.bind(this));

    this.fade = this.fade.bind(this);
    this.fadeIn = this.fadeIn.bind(this);
  }

  createClass(Cursor, [
    {
      key: 'initialAssignment',
      value: function initialAssignment() {
        Object.assign(this.el.style, {
          opacity: '1',
          'transition-duration': '0.1s',
        });
      },
    },
    {
      key: 'fade',
      value: function fade() {
        this.el.style.opacity = '0';

        this.faded = true;
      },
    },
    {
      key: 'fadeIn',
      value: function fadeIn() {
        this.el.style.opacity = '1';

        this.faded = false;
      },
    },
    {
      key: 'logic',
      value: function logic() {
        this.faded ? setTimeout(this.fadeIn, this.speed) : setTimeout(this.fade, this.speed);
      },
    },
    {
      key: 'start',
      value: function start() {
        setTimeout(this.fade.bind(this), 0);
      },
    },
  ]);
  return Cursor;
})();

var Typewriter = (function () {
  function Typewriter(el, options) {
    classCallCheck(this, Typewriter);

    this.el = el;
    this.text = '';
    this.queue = [];
    this.options = Object.assign({}, defaultOptions, options);

    this.createTextEl();
  }

  // USER API

  createClass(Typewriter, [
    {
      key: 'type',
      value: function type(str) {
        this.queue.push({
          type: 'type',
          content: str,
        });

        return this;
      },
    },
    {
      key: 'strings',
      value: function strings(interval) {
        var _this = this;

        for (var _len = arguments.length, arr = Array(_len > 1 ? _len - 1 : 0), _key = 1; _key < _len; _key++) {
          arr[_key - 1] = arguments[_key];
        }

        arr.forEach(function (str, i) {
          _this.queue.push({
            type: 'type',
            content: str,
          });

          if (interval) {
            _this.queue.push({
              type: 'pause',
              time: interval,
            });
          }

          if (i === arr.length - 1) return;

          _this.queue.push({
            type: 'deleteChars',
            count: str.length,
          });
        });

        return this;
      },
    },
    {
      key: 'remove',
      value: function remove(num) {
        this.queue.push({
          type: 'deleteChars',
          count: num,
        });

        return this;
      },
    },
    {
      key: 'clear',
      value: function clear() {
        this.queue.push({
          type: 'clear',
        });

        return this;
      },
    },
    {
      key: 'clearText',
      value: function clearText() {
        this.text = '';
        this.render();

        return this;
      },
    },
    {
      key: 'queueClearText',
      value: function queueClearText() {
        this.queue.push({
          type: 'clearText',
        });

        return this;
      },
    },
    {
      key: 'clearQueue',
      value: function clearQueue() {
        this.queue = [];
        this.text = '';
        render();

        return this;
      },
    },
    {
      key: 'rest',
      value: function rest(time) {
        this.queue.push({
          type: 'pause',
          time: time,
        });

        return this;
      },
    },
    {
      key: 'changeOps',
      value: function changeOps(options) {
        this.queue.push({
          type: 'changeOps',
          options: options,
        });

        return this;
      },
    },
    {
      key: 'then',
      value: function then(cb) {
        this.queue.push({
          type: 'callback',
          cb: cb,
        });

        return this;
      },
    },
    {
      key: 'removeCursor',
      value: function removeCursor() {
        this.queue.push({
          type: 'deleteCursor',
        });

        return this;
      },
    },
    {
      key: 'addCursor',
      value: function addCursor() {
        this.queue.push({
          type: 'createCursor',
        });

        return this;
      },
    },
    {
      key: 'changeTypeColor',
      value: function changeTypeColor(color) {
        this.queue.push({
          type: 'typeColor',
          color: color,
        });

        return this;
      },
    },
    {
      key: 'changeCursorColor',
      value: function changeCursorColor(color) {
        this.queue.push({
          type: 'cursorColor',
          color: color,
        });

        return this;
      },
    },
    {
      key: 'changeTypeClass',
      value: function changeTypeClass(className) {
        this.queue.push({
          type: 'typeClass',
          className: className,
        });

        return this;
      },
    },
    {
      key: 'changeCursorClass',
      value: function changeCursorClass(className) {
        this.queue.push({
          type: 'cursorClass',
          className: className,
        });

        return this;
      },
    },
    {
      key: 'start',
      value: function start() {
        var _this2 = this;

        if (this.running) return;

        if (!this.cursorEl) {
          this.createCursorEl();
        }

        this.running = true;
        this.deleteAll().then(function (_) {
          return _this2.loop(0);
        });
      },

      // ACTIONS (promises)
    },
    {
      key: 'add',
      value: function add(content) {
        var _this3 = this;

        var count = 0;
        this.timestamp = Date.now();

        return new Promise(function (resolve, _) {
          var _step = function _step() {
            if (count === content.length) return resolve();

            var newStamp = Date.now();
            var change = newStamp - _this3.timestamp;

            if (change >= _this3.getTypeSpeed()) {
              _this3.addChar(content[count]);
              _this3.timestamp = newStamp;
              count++;
            }
            requestAnimationFrame(_step);
          };

          requestAnimationFrame(_step);
        });
      },
    },
    {
      key: 'delete',
      value: function _delete(count) {
        var _this4 = this;

        this.timestamp = Date.now();

        return new Promise(function (resolve, _) {
          var _step = function _step() {
            if (count === 0) return resolve();

            var newStamp = Date.now();
            var change = newStamp - _this4.timestamp;

            if (change >= _this4.getDeleteSpeed()) {
              _this4.deleteChar();
              _this4.timestamp = newStamp;
              count--;
            }
            requestAnimationFrame(_step);
          };

          requestAnimationFrame(_step);
        });
      },
    },
    {
      key: 'deleteAll',
      value: function deleteAll() {
        return this.delete(this.text.length);
      },
    },
    {
      key: 'pause',
      value: function pause(time) {
        return new Promise(function (resolve, _) {
          setTimeout(resolve, time);
        });
      },
    },
    {
      key: 'callback',
      value: function callback(cb) {
        return new Promise(function (resolve, _) {
          cb();
          resolve();
        });
      },
    },
    {
      key: 'deleteCursor',
      value: function deleteCursor() {
        var _this5 = this;

        return new Promise(function (resolve, _) {
          _this5.removeCursorEl();
          resolve();
        });
      },
    },
    {
      key: 'createCursor',
      value: function createCursor() {
        var _this6 = this;

        return new Promise(function (resolve, _) {
          _this6.createCursorEl();
          resolve();
        });
      },
    },
    {
      key: 'clearTextAction',
      value: function clearTextAction() {
        var _this7 = this;

        return new Promise(function (resolve, _) {
          _this7.clearText();
          resolve();
        });
      },
    },
    {
      key: 'changeOpsAction',
      value: function changeOpsAction(options) {
        var _this8 = this;

        return new Promise(function (resolve, _) {
          _this8.options = Object.assign(_this8.options, options);
          resolve();
        });
      },
    },
    {
      key: 'typeColor',
      value: function typeColor(color) {
        var _this9 = this;

        return new Promise(function (resolve, _) {
          _this9.textEl.style.color = color;
          resolve();
        });
      },
    },
    {
      key: 'cursorColor',
      value: function cursorColor(color) {
        var _this10 = this;

        return new Promise(function (resolve, _) {
          _this10.cursorEl.style.color = color;
          resolve();
        });
      },
    },
    {
      key: 'typeClass',
      value: function typeClass(className) {
        var _this11 = this;

        return new Promise(function (resolve, _) {
          _this11.textEl.className = className;
          resolve();
        });
      },
    },
    {
      key: 'cursorClass',
      value: function cursorClass(className) {
        var _this12 = this;

        return new Promise(function (resolve, _) {
          _this12.cursorEl.className = className;
          resolve();
        });
      },

      // HELPERS
    },
    {
      key: 'deleteChar',
      value: function deleteChar() {
        this.text = this.text.slice(0, -1);
        this.render();
      },
    },
    {
      key: 'addChar',
      value: function addChar(char) {
        this.text += char;
        this.render();
      },
    },
    {
      key: 'getTypeSpeed',
      value: function getTypeSpeed() {
        var speed = this.options.typeSpeed;

        if (typeof speed === 'number') {
          return speed;
        }

        var max = this.options.typeSpeedMax;
        var min = this.options.typeSpeedMin;

        var random = Math.floor(Math.random() * (max - min));
        return random + min;
      },
    },
    {
      key: 'getDeleteSpeed',
      value: function getDeleteSpeed() {
        var speed = this.options.deleteSpeed;

        if (typeof speed === 'number') {
          return speed;
        }

        var max = this.options.deleteSpeedMax;
        var min = this.options.deleteSpeedMin;

        var random = Math.floor(Math.random() * (max - min));
        return random + min;
      },
    },
    {
      key: 'step',
      value: function step(idx) {
        var action = this.queue[idx];

        switch (action.type) {
          case 'type':
            return this.add(action.content);

          case 'deleteChars':
            return this.delete(action.count);

          case 'clear':
            return this.deleteAll();

          case 'pause':
            return this.pause(action.time);

          case 'callback':
            return this.callback(action.cb);

          case 'deleteCursor':
            return this.deleteCursor();

          case 'createCursor':
            return this.createCursor();

          case 'clearText':
            return this.clearTextAction();

          case 'changeOps':
            return this.changeOpsAction(action.options);

          case 'typeColor':
            return this.typeColor(action.color);

          case 'cursorColor':
            return this.cursorColor(action.color);

          case 'typeClass':
            return this.typeClass(action.className);

          case 'cursorClass':
            return this.cursorClass(action.className);
        }
      },
    },
    {
      key: 'loop',
      value: function loop(idx) {
        var _this13 = this;

        if (idx === this.queue.length) {
          this.running = false;

          if (this.options.loop) {
            this.start();
          }
          return;
        }

        this.step(idx).then(function (_) {
          _this13.loop(idx + 1);
        });
      },
    },
    {
      key: 'createCursorEl',
      value: function createCursorEl() {
        if (typeof this.options.animateCursor === 'String') return;

        this.cursorEl = document.createElement('span');
        this.cursorEl.innerHTML = '|';

        this.cursorEl.style.color = this.options.cursorColor;

        this.cursorEl.classList.add(this.options.cursorClass);

        this.el.appendChild(this.cursorEl);

        if (this.options.animateCursor) {
          this.cursor = new Cursor(this.cursorEl, this.options.blinkSpeed);

          this.cursor.start();
        }
      },
    },
    {
      key: 'removeCursorEl',
      value: function removeCursorEl() {
        this.el.removeChild(this.cursorEl);

        this.cursorEl = null;
      },
    },
    {
      key: 'createTextEl',
      value: function createTextEl() {
        this.textEl = document.createElement('div');

        this.textEl.classList.add(this.options.typeClass);
        this.textEl.classList.add('glitch');
        // this.textEl.classList.add("glitch");
        this.textEl.style.color = this.options.typeColor;

        this.el.appendChild(this.textEl);
      },
    },
    {
      key: 'render',
      value: function render() {
        this.textEl.innerHTML = this.text;
        this.textEl.setAttribute('data-text', this.text);
        this.textEl.classList.add('force-redraw');
        this.textEl.classList.remove('force-redraw');
      },
    },
  ]);
  return Typewriter;
})();

const options = {
  loop: false,
  typeColor: 'white',
  typeSpeed: 300,
};
const target = document.querySelector('#t-writer-title');
const writer = new Typewriter(target, options);

'ENOWARS 5'.split('').forEach((char) => {
  writer.type(char).rest(Math.random() * 30);
});

writer.start();
