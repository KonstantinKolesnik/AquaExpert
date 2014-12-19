
define(
	//['app', 'marionette', 'backbone', 'underscore', 
	// 'text!myplugin/layout-template.tpl',	// общий шаблон страницы
	// 'text!myplugin/list-template.tpl',	// шаблон для списка объектов
	// 'text!myplugin/item-template.tpl'	// шаблон для элемента списка
	//],
	['app', 'marionette', 'backbone', 'underscore', 'text!webapp/mysensors/templates.tpl'],

	//function (application, marionette, backbone, _, tmplLayout, tmplList, tmplListItem) {
    function (application, marionette, backbone, _, templates) {
	    //var myLayout = marionette.LayoutView.extend({
	    //    template: _.template(tmplLayout),
				
        //});

        //Id: "31249bdb-1649-45b3-ac9c-89db4b88ef63"
        //Name: null
        //NodeNo: 1
        //ProtocolVersion: "1.4.1"
        //SketchName: "Power switch 8"
        //SketchVersion: "1.0"
        //Type: 17
	    var mNodeView = marionette.ItemView.extend({
	        //template: _.template(tmplListItem),
	        //template: _.template('[# <%= NodeNo %>] [Type: <%= Type %>] [Sketch name: <%= SketchName %>] [Sketch version: <%= SketchVersion %>] [Protocol version: <%= ProtocolVersion %>]')
	        template: _.template(templates)
	    });

	    var mNodesView = marionette.CompositeView.extend({
	        //template: _.template(tmplList),
	        template: _.template('<h1>Узлы</h1><div id="lstNodes"></div>'),
	        childView: mNodeView,
	        childViewContainer: '#lstNodes'
	    });

	    return {
	        //layout: myLayout,
	        nodesView: mNodesView
	    };
	});