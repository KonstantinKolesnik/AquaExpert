
function GridStateManager(gridID) {
    var me = this;
    var expandedItems = new ItemCollection();
    var scrollPosition = null;
    var scrollPositionGrid = null;
    var saveScroll = true;
    var scrollBound = false;

    $(window).on("scroll", saveScrollState);

    me.onDetailExpand = function (e) {
        addToVisualState(e.masterRow);
    }
    me.onDetailCollapse = function (e) {
        removeFromVisualState(e.masterRow);
    }
    me.onDataBound = function () {
        restoreVisualState();
        if (!scrollBound) {
            $("#" + gridID).find(".k-grid-content").on("scroll", saveScrollState);
            scrollBound = true;
        }
    }

    function saveScrollState() {
        if (saveScroll) {
            scrollPosition = $(window).scrollTop();
            scrollPositionGrid = $("#" + gridID).find(".k-grid-content").scrollTop();
        }
    }
    function restoreScrollState() {
        if (scrollPosition)
            $(window).scrollTop(scrollPosition);
        if (scrollPositionGrid)
            $("#" + gridID).find(".k-grid-content").scrollTop(scrollPositionGrid);
    }

    function addToVisualState(row) {
        if (!expandedItems.restoring) {
            var item = row.closest(".k-grid").data("kendoGrid").dataItem(row);
            expandedItems.add(item);
        }
    }
    function removeFromVisualState(row) {
        var item = row.closest(".k-grid").data("kendoGrid").dataItem(row);
        expandedItems.remove(item);
    }
    function restoreVisualState() {
        var sp = scrollPosition;
        var spGrid = scrollPositionGrid;

        if (expandedItems.restoring)
            return;
        else
            expandedItems.restoring = true;

        var rows = null; //cache for performance
        expandedItems.each(function (itemID) {
            if (!rows)
                rows = $("#" + gridID + " tr.k-master-row");

            for (var i = 0; i < rows.length; i++) {
                var row = rows[i];
                var jRow = $(row);
                var grid = jRow.closest("#" + gridID).data("kendoGrid");
                var data = grid.dataItem(jRow);
                if (data.ID == itemID) {
                    grid.expandRow(row);
                    rows = null;
                    break;
                }
            }
        });

        expandedItems.restoring = false;

        // restore vertical scroll position;
        scrollPosition = sp;
        scrollPositionGrid = spGrid;
        restoreScrollState();
    }

    function ItemCollection() {
        var me = this;
        var items = [];

        me.add = function (item) {
            if (items.indexOf(item.ID) == -1)
                items.push(item.ID);
        }
        me.remove = function (item) {
            var i = items.indexOf(item.ID);

            if (i != -1)
                items.splice(i, 1);
        }
        me.has = function (item) {
            return items.indexOf(item.ID) != -1;
        }
        me.empty = function () {
            items.length = 0;
        }
        me.each = function (fn) {
            items.forEach(function (item) { fn(item); });
        }
    }
}