﻿var $url = '/common/userLayerView';

var data = utils.init({
  guid: utils.getQueryString('guid'),
  user: null,
  groupName: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        guid: this.guid
      }
    }).then(function (response) {
      var res = response.data;

      $this.user = res.user;
      $this.groupName = res.groupName;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnCancelClick: function () {
    utils.closeLayer();
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(null, this.btnCancelClick);
    this.apiGet();
  }
});
