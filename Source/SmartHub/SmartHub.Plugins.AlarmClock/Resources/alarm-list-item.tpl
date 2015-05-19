<div class="th-margin-bottom-32">
		<h3>
			<a href="#" 
				class="btn th-btn-width-120 pull-right js-btn-enable"
				data-action-text="Включить"
				data-action-class="btn-primary"
				data-state-class="btn-default">Выключен</a>

			<a href="#" 
				class="btn th-btn-width-120 pull-right js-btn-disable"
				data-action-text="Выключить"
				data-action-class="btn-danger"
				data-state-class="btn-success">Включен</a>
			<a href="#" class="js-btn-edit">
				<%=hours%>:<%=('0' + minutes).slice(-2)%>
			</a>
		</h3>
		<h4>
			<a href="#" class="js-btn-edit">
				<%=name%>
			</a>
		</h4>
		<p class="js-run-script">
			Запуск скрипта: <em><%= scriptName %></em>.
		</p>
		<p class="js-play-sound">
			Звуковой сигнал
		</p>
</div>
