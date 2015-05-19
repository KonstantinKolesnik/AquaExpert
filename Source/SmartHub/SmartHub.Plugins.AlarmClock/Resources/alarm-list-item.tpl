<div>
  <hr/>
  <i class="fa fa-bell-o fa-3x" style="float:left; margin-right:20px;"></i>

  <div class="th-margin-bottom-20">
		  <h4>
			  <a href="#" 
				  class="btn th-btn-width-120 pull-right js-btn-enable"
				  data-action-text="Включить"
				  data-action-class="btn-primary"
				  data-state-class="btn-default">Неактивно</a>

			  <a href="#" 
				  class="btn th-btn-width-120 pull-right js-btn-disable"
				  data-action-text="Выключить"
				  data-action-class="btn-danger"
				  data-state-class="btn-success">Активно</a>
      
			  <a href="#" class="js-btn-edit"><%=hours%>:<%=('0' + minutes).slice(-2)%></a>
		  </h4>
  
		  <h4><a href="#" class="js-btn-edit"><%=name%></a></h4>
      <div class="js-run-script">Скрипт "<em><%= scriptName %>"</em>.</div>
      <div class="js-play-sound">Звуковой сигнал</div>
  </div>
</div>