
define(
	['app', 'marionette', 'backbone', 'underscore', 'kendo', 'text!webapp/mysensors/templates.html'],
    function (application, marionette, backbone, _, kendo, templates) {
	    //var mLayout = marionette.LayoutView.extend({
	    //    //template: _.template(tmplLayout),
	    //    template: _.template('<h1>MySensors network</h1><div id="region-list"></div>' +
        //        '<select id="size">' + //data-role="kendodropdownlist"
        //        '<option>S - 6 3/4"</option>'+
        //        '<option>M - 7 1/4"</option>'+
        //        '<option>L - 7 1/8"</option>'+
        //        '<option>XL - 7 5/8"</option>' +
        //    '</select>'
        //        ),
	    //    regions: {
	    //        //filter: '#region-filter',
	    //        list: '#region-list'
	    //    }
	    //});

        var mLayout = marionette.LayoutView.extend({
            template: _.template(templates),
            //onShow: function () {
            //    //debugger;
            //    //this.$el.kendoPanelBar({
            //    //    expandMode: "single"
            //    //});
            //    $("#size").kendoDropDownList();
            //}
        });






        //Id: "31249bdb-1649-45b3-ac9c-89db4b88ef63"
        //Name: null
        //NodeNo: 1
        //ProtocolVersion: "1.4.1"
        //SketchName: "Power switch 8"
        //SketchVersion: "1.0"
        //Type: 17
	    //var mNodeView = marionette.ItemView.extend({
	    //    //template: _.template(tmplListItem),
	    //    //template: _.template('[# <%= NodeNo %>] [Type: <%= Type %>] [Sketch name: <%= SketchName %>] [Sketch version: <%= SketchVersion %>] [Protocol version: <%= ProtocolVersion %>]')
	    //    template: _.template(templates)
	    //});

	    //var mNodesView = marionette.CompositeView.extend({
	    //    //template: _.template(tmplList),
	    //    template: _.template('<h3>Узлы</h3><div id="lstNodes"></div>'),
	    //    childView: mNodeView,
	    //    childViewContainer: '#lstNodes',
	    //    onShow: function () {
	    //        //debugger;
	    //        //this.$el.kendoPanelBar({
	    //        //    expandMode: "single"
	    //        //});

	    //        $("#size").kendoDropDownList();
	    //    }
	    //});

	    return {
	        layoutView: mLayout
	        //nodesView: mNodesView
	    };
	});