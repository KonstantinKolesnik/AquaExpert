<form>
  <div class="row">
    <div class="col-md-4">
      <h2>Редактирование оповещения</h2>
      <p>
        <input type="button" value="Сохранить" class="btn btn-primary js-btn-save" />&nbsp;
        <input type="button" value="Отмена" class="btn btn-default js-btn-cancel" />
        <input type="button" value="Удалить" class="btn btn-danger pull-right js-btn-delete" />
      </p>
      <div class="form-group">
        <label>Имя</label>
        <input name="name" class="form-control" />
      </div>
      <div class="form-group">
        <label>Время</label>
        <ul class="list-inline">
          <li>
            <select name="hours" class="form-control">
              <% for (var h = 0; h < 24; h++) { %>
              <option value="<%= h %>"><%= ('0' + h).slice(-2) %></option>
              <% } %>
            </select>
          </li>
          <li>
            <select name="minutes" class="form-control">
              <% for (var m = 0; m < 60; m++) { %>
              <option value="<%= m %>"><%= ('0' + m).slice(-2) %></option>
              <% } %>
            </select>
          </li>
        </ul>
      </div>
      <div class="form-group">
        <label>Действие</label>
        <select name="scriptId" data-items-field="scripts" class="form-control">
          <option value="">&lt;Звуковой сигнал&gt;</option>
        </select>
      </div>
    </div>
  </div>
</form>
