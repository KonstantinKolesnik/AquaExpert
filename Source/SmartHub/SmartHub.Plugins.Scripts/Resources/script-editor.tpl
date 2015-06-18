<form>
  <h2>
    <%= Name %>
  </h2>
  <div class="form-group">
    <label>Имя</label>
    <input name="Name" class="form-control" />
  </div>
  
  <p>
    <input type="button" value="Сохранить" class="btn btn-primary js-btn-save" />&nbsp;
    <input type="button" value="Отмена" class="btn btn-default js-btn-cancel" />
  </p>
  <div class="th-margin-bottom-6">
    <textarea name="Body" class="js-script-body " />
  </div>
  <div class="cm-s-bootstrap-dark js-editor-panel">
    <a href="#" class="btn btn-default btn-xs js-full-screen">Полноэкранный режим</a>
    <a href="#" class="btn btn-default btn-xs js-exit-full-screen hidden">Выход из полноэкранного режима</a>
  </div>
</form>
