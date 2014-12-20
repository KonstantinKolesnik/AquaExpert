
define(
	['app', 'marionette', 'backbone', 'underscore', 'webapp/mysensors/views2', 'text!webapp/mysensors/templates2.html'],//, 'text!webapp/mysensors/css/styles2.css'],
	function (application, marionette, backbone, _, views, templates) {//, styles) {
	    debugger;

        var res = {
            start: function () {
                // View Definitions
                // ----------------
                debugger;
                // Define an Book View to render the individual book
                var BookView = Marionette.ItemView.extend({
                    className: "book",
                    template: "#book-view-template"
                });

                // Define a Category View which will render the book
                // Categories as panels, rendering the collection of book
                // for each category within that panel.
                //
                // A Marionette.CompositeView is a view that can render
                // both a model with template, and a collection of models
                // that get placed within the rendered template.
                var CategoryView = Marionette.CompositeView.extend({
                    // the HTML tag name for this view
                    tagName: "li",
                    // the template to render for the category
                    template: "#category-panel-template",
                    // the view type to render for the collection of books
                    itemView: BookView,
                    // where to place the list of books, within the rendered template
                    itemViewContainer: ".book-list",
                    initialize: function () {
                        // get the list of books for this category and assign it to
                        // this view's "collection" so that it will render as the
                        // specified itemView instance
                        this.collection = this.model.books;
                    }
                });

                // Define a Category List view that will render the 
                // collection of categories as CategoryView instances
                var CategoryListView = Marionette.CollectionView.extend({
                    // render each model in to a category view
                    childView: CategoryView,
                    // tell to to render a "ul" tag as the wrapper
                    tagName: "ul",
                    // after it's all rendered and has been added to the DOM,
                    // initialize the KendoUI PanelBar
                    onShow: function () {
                        this.$el.kendoPanelBar({
                            expandMode: "single"
                        });
                    }
                });


                // Model And Collection Definitions
                // --------------------------------

                // Define a Book model to hold info about books
                var Book = Backbone.Model.extend({});
                // Define a collection of books
                var BookCollection = Backbone.Collection.extend({
                    // tell it to use the Book as the type of model
                    model: Book
                });
                // Define a category model to hold a collection of books
                var Category = Backbone.Model.extend({});
                // Define a category collection to hold a list of categories
                var CategoryCollection = Backbone.Collection.extend({
                    model: Category
                });

                // Book And Category Data Structure And Instances
                // ----------------------------------------------
                var scifiBooks = new BookCollection([
                    { id: 1, title: "Neuromancer", author: "William Gibson" },
                    { id: 2, title: "Snow Crash", author: "Neal Stephenson" }
                ]);
                var scifiCategory = new Category({ name: "Science Fiction" });
                scifiCategory.books = scifiBooks;

                var vampireBooks = new BookCollection([
                    { id: 3, title: "Abraham Lincon: Vampire Hunter", author: "Seth Grahame-Smith" },
                    { id: 4, title: "Interview With A Vampire", author: "Ann Rice" }
                ]);
                var vampireCategory = new Category({ name: "Vampires" });
                vampireCategory.books = vampireBooks;


                // build a collection of categories
                var categories = new CategoryCollection([scifiCategory, vampireCategory]);

                // Application Initialization
                // --------------------------
                var mLayout = marionette.LayoutView.extend({
                    //template: _.template(tmplLayout),
                    template: _.template('<h1>Test</h1><div id="placeholder"></div>'),
                    regions: {
                        list: '#placeholder'
                    }
                });
                var lll = new mLayout();
                application.setContentView(lll);



                var categoryListView = new CategoryListView({ collection: categories });
                //var region = new Marionette.Region({ el: "#placeholder" });
                //region.show(categoryListView);
                lll.list.show(categoryListView);
            }
        }

        return res;
	});