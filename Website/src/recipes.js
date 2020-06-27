import React, { Component } from "react";
import { Link } from "react-router-dom";
import axios from "axios";
import "./dashboard.css";
import Swal from "sweetalert2";
import RecipesTable from "./components/RecipesTable";

export default class Recipes extends Component {
  constructor(props) {
    super(props);

    this.state = {
      search: "",
      recipes: [{}],
      name: null,
    };
  }

  changeHandler = (e) => {
    this.setState({ [e.target.name]: e.target.value });
  };

  submitHandler = (e) => {
    e.preventDefault();
    if (this.state.search) {
      let token = localStorage.getItem("auth");
      const form = new FormData();

      let search = this.state.search;

      form.append("keys", search);
      axios
        .post("http://localhost:1331/recipe/search", form, {
          headers: { "Content-Type": "multipart/form-data", auth: token},
          withCredentials: true,
        })
        .then((response) => {
          this.setState({ recipes: response.data });
          console.log(response.data)
          this.showRecipes();
        })
        .catch((error) => {
          console.log(error);

          // REMOVER
          this.showRecipes();

        });
    }
  };

  getInfo = () => {
    let token = localStorage.getItem("auth");
    axios
      .get(
        "http://localhost:1331/user/info",
        {
          headers: { auth: token },
        },
        { withCredentials: true }
      )
      .then((response) => {
        let json = response.data;
        this.setState({
          name: json["_name"],
        });
      })
      .catch((error) => {
        console.log(error);
      });

    if (localStorage.getItem("auth") != null) {
      return true;
    } else {
      return false;
    }
  };

  removeToken = () => {
    localStorage.removeItem("auth");
    this.props.history.push("/");
    window.location.reload();
  };

  collapseBar() {
    if (document.body.className === "") {
      document.body.className = "collapsed";
    } else {
      document.body.className = "";
    }
  }

  showRecipes = () => {
    var x;
    //var json = this.state.recipes;
    var json = [{"title":"Test Pilot","image":"https://spoonacular.com/recipeImages/206856-556x370.jpg","id":206856,"extendedIngredients":[{"id":"93653","name":"angostura bitters","original":"1 dash Angostura bitters"},{"id":"10014412","name":"ice","original":"1 cup crushed ice"},{"id":"9160","name":"lime juice","original":"1/2 ounce fresh lime juice"},{"id":"14551","name":"pernod","original":"6 drops Pernod"},{"id":"14037","name":"rum","original":"3/4 ounce light Puerto Rican rum"},{"id":"14037","name":"rum","original":"1 1/2 ounces dark Jamaican rum"},{"id":null,"name":"cointreau","original":"1 tablespoon Cointreau"}],"summary":"Test Pilot might be just the beverage you are searching for. This gluten free, fodmap friendly, and vegan recipe serves 1 and costs <b>$2.03 per serving</b>. One serving contains <b>171 calories</b>, <b>0g of protein</b>, and <b>0g of fat</b>. This recipe from Serious Eats has 21 fans. Head to the store and pick up angostura bitters, pernod, jamaican rum, and a few other things to make it today. From preparation to the plate, this recipe takes roughly <b>3 minutes</b>. All things considered, we decided this recipe <b>deserves a spoonacular score of 7%</b>. This score is improvable. Try <a href=\"https://spoonacular.com/recipes/this-is-a-test-517956\">This Is A Test…</a>, <a href=\"https://spoonacular.com/recipes/fresh-vs-canned-pumpkin-i-put-them-to-the-test-558628\">Fresh vs. Canned Pumpkin: I put them to the test</a>, and <a href=\"https://spoonacular.com/recipes/meatless-chicken-shawarma-taste-test-684139\">Meatless Chicken Shawarma Taste Test</a> for similar recipes."},{"title":"America's Test Kitchen Chocolate Chip Cookies","image":"https://spoonacular.com/recipeImages/626421-556x370.png","id":626421,"extendedIngredients":[{"id":"18372","name":"baking soda","original":"1/2 teaspoon baking soda"},{"id":"1001","name":"butter","original":"14 tablespoons butter, divided"},{"id":"10019146","name":"chocolate chips","original":"1 1/4 cup chocolate chips"},{"id":"10019334","name":"dark brown sugar","original":"3/4 cup dark brown sugar"},{"id":"1123","name":"egg","original":"1 large egg + 1 large egg yolk"},{"id":"20081","name":"flour","original":"1 3/4 cup all-purpose flour"},{"id":"19335","name":"granulated sugar","original":"1/2 cup granulated sugar"},{"id":"2047","name":"salt","original":"1 teaspoon salt"},{"id":"2050","name":"vanilla extract","original":"2 teaspoons vanilla extract"}],"summary":"America's Test Kitchen Chocolate Chip Cookies might be just the dessert you are searching for. One serving contains <b>185 calories</b>, <b>2g of protein</b>, and <b>9g of fat</b>. For <b>21 cents per serving</b>, this recipe <b>covers 2%</b> of your daily requirements of vitamins and minerals. Not a lot of people made this recipe, and 8 would say it hit the spot. If you have granulated sugar, brown sugar, egg, and a few other ingredients on hand, you can make it. From preparation to the plate, this recipe takes around <b>45 minutes</b>. All things considered, we decided this recipe <b>deserves a spoonacular score of 10%</b>. This score is very bad (but still fixable). Try <a href=\"https://spoonacular.com/recipes/maple-soy-glazed-salmon-americas-test-kitchen-86915\">Maple-Soy Glazed Salmon (America's Test Kitchen)</a>, <a href=\"https://spoonacular.com/recipes/americas-test-kitchen-slow-cooker-beef-burgundy-143455\">America's Test Kitchen Slow Cooker Beef Burgundy</a>, and <a href=\"https://spoonacular.com/recipes/sweet-chili-glazed-tofu-with-bok-choy-americas-test-kitchen-92091\">Sweet Chili-Glazed Tofu With Bok Choy - America's Test Kitchen</a> for similar recipes."},{"title":"America's Test Kitchen Slow Cooker Beef Burgundy","image":"https://spoonacular.com/recipeImages/143455-556x370.jpg","id":143455,"extendedIngredients":[{"id":"10123","name":"bacon","original":"8 ounces bacon, chopped"},{"id":"2004","name":"bay leaves","original":"3 bay leaves"},{"id":"13786","name":"beef chuck roast","original":"4 lbs beef chuck, cut into 1 1/2 inch chunks"},{"id":"11260","name":"button mushrooms","original":"8 ounces sliced button mushrooms"},{"id":"11124","name":"carrots","original":"2 carrots, peeled and chopped fine"},{"id":"11297","name":"fresh parsley","original":"3 tablespoons minced fresh parsley"},{"id":"2049","name":"fresh thyme","original":"2 teaspoons chopped fresh thyme"},{"id":"11215","name":"garlic cloves","original":"8 garlic cloves, minced"},{"id":"6970","name":"low sodium chicken broth","original":"1 1/2 cups reduced-sodium chicken broth"},{"id":"93776","name":"minute tapioca","original":"3 tablespoons minute tapioca"},{"id":"11282","name":"onion","original":"1 large onion, chopped fine"},{"id":"10111282","name":"pearl onions","original":"8 ounces white pearl onions"},{"id":"14099","name":"pinot noir","original":"2 1/2 cups pinot noir wine"},{"id":"1102047","name":"Salt & Pepper","original":"salt & freshly ground black pepper"},{"id":"16124","name":"soy sauce","original":"1/3 cup soy sauce"},{"id":"11887","name":"tomato paste","original":"4 tablespoons tomato paste"}],"summary":"America's Test Kitchen Slow Cooker Beef Burgundy is a <b>gluten free and dairy free</b> main course. This recipe serves 6 and costs $5.97 per serving. One portion of this dish contains around <b>68g of protein</b>, <b>50g of fat</b>, and a total of <b>875 calories</b>. A couple people made this recipe, and 19 would say it hit the spot. A mixture of carrots, chicken broth, pinot noir wine, and a handful of other ingredients are all it takes to make this recipe so yummy. To use up the bacon you could follow this main course with the <a href=\"https://spoonacular.com/recipes/blueberry-buckle-51636\">Blueberry Buckle</a> as a dessert. From preparation to the plate, this recipe takes roughly <b>9 hours and 15 minutes</b>. All things considered, we decided this recipe <b>deserves a spoonacular score of 85%</b>. This score is amazing. Similar recipes include <a href=\"https://spoonacular.com/recipes/macaroni-and-cheese-in-the-pressure-cooker-and-an-americas-test-kitchen-cookbook-giveaway-487246\">Macaroni and Cheese in the Pressure Cooker and an America’s Test Kitchen Cookbook Giveaway</a>, <a href=\"https://spoonacular.com/recipes/americas-test-kitchen-chocolate-chip-cookies-626421\">America's Test Kitchen Chocolate Chip Cookies</a>, and <a href=\"https://spoonacular.com/recipes/maple-soy-glazed-salmon-americas-test-kitchen-86915\">Maple-Soy Glazed Salmon (America's Test Kitchen)</a>."},{"title":"The Chocolate Milk Test {: Chocolate Silk Pancakes}","image":"https://spoonacular.com/recipeImages/509849-556x370.jpg","id":509849,"extendedIngredients":[{"id":"18371","name":"baking powder","original":"1 teaspoon baking powder"},{"id":"18372","name":"baking soda","original":"1/2 teaspoon baking soda"},{"id":"1001","name":"butter","original":"2 Tablespoons butter, melted"},{"id":"1102","name":"chocolate milk","original":"1 cup Chocolate Silk Soymilk or Chocolate Milk"},{"id":"1123","name":"egg","original":"1 egg"},{"id":"20081","name":"flour","original":"1 cup all-purpose flour"},{"id":"1082047","name":"kosher salt","original":"1/2 teaspoon Kosher salt"},{"id":"42135","name":"whipped topping","original":"Whipped Topping, diced fresh strawberries and powdered sugar for garnish"},{"id":"19335","name":"white sugar","original":"2 tablespoons white sugar"},{"id":"2053","name":"white vinegar","original":"2 Tablespoons white vinegar"}],"summary":"The Chocolate Milk Test {: Chocolate Silk Pancakes} might be just the beverage you are searching for. One serving contains <b>136 calories</b>, <b>3g of protein</b>, and <b>5g of fat</b>. For <b>21 cents per serving</b>, this recipe <b>covers 4%</b> of your daily requirements of vitamins and minerals. Several people made this recipe, and 5686 would say it hit the spot. Head to the store and pick up baking powder, egg, sugar, and a few other things to make it today. From preparation to the plate, this recipe takes about <b>18 minutes</b>. All things considered, we decided this recipe <b>deserves a spoonacular score of 33%</b>. This score is not so excellent. Try <a href=\"https://spoonacular.com/recipes/chocolate-milk-pancakes-79132\">Chocolate Milk Pancakes</a>, <a href=\"https://spoonacular.com/recipes/mostly-homemade-mom-chocolate-milk-pancakes-631202\">Mostly Homemade Mom: Chocolate Milk Pancakes</a>, and <a href=\"https://spoonacular.com/recipes/trumoo-chocolate-milk-pancakes-584976\">TruMoo Chocolate Milk Pancakes</a> for similar recipes."},{"title":"Meatless Chicken Shawarma Taste Test","image":"https://spoonacular.com/recipeImages/684139-556x370.jpg","id":684139,"extendedIngredients":[{"id":"2001","name":"all spice","original":"½ teaspoon all spice"},{"id":"1015006","name":"chicken meat","original":"1 pack Beyond Meat Chicken strips"},{"id":"2010","name":"cinnamon","original":"¼ teaspoon cinnamon"},{"id":"1002014","name":"cumin","original":"1 teaspoon cumin"},{"id":"11215","name":"garlic","original":"2 cloves garlic, diced"},{"id":"1022020","name":"garlic powder","original":"½ teaspoon garlic powder"},{"id":"4053","name":"olive oil","original":"4 tablespoons olive oil ( 2 for the pan and 2 in the paste)"},{"id":"11282","name":"onion","original":"½ cup sliced onion"},{"id":"2028","name":"paprika","original":"½ teaspoon paprika"},{"id":"1002030","name":"pepper","original":"¼ teaspoon pepper"},{"id":"18413","name":"pitas","original":"4 pitas"},{"id":"2047","name":"salt","original":"½ teaspoon salt"},{"id":"11887","name":"tomato paste","original":"1 tablespoon tomato paste"},{"id":"1116","name":"yogurt","original":"Creamy Yogurt White Sauce (omit for vegan version)"}],"summary":"You can never have too many main course recipes, so give Meatless Chicken Shawarma Taste Test a try. For <b>$3.05 per serving</b>, this recipe <b>covers 29%</b> of your daily requirements of vitamins and minerals. This recipe makes 4 servings with <b>1016 calories</b>, <b>68g of protein</b>, and <b>65g of fat</b> each. 14 people have made this recipe and would make it again. Head to the store and pick up pack beyond meat chicken strips, salt, paprika, and a few other things to make it today. To use up the cinnamon you could follow this main course with the <a href=\"https://spoonacular.com/recipes/cinnamon-fudge-836311\">Cinnamon Fudge</a> as a dessert. All things considered, we decided this recipe <b>deserves a spoonacular score of 48%</b>. This score is solid. Try <a href=\"https://spoonacular.com/recipes/shawarma-djaj-chicken-shawarma-lebanon-middle-east-102838\">Shawarma Djaj -- Chicken Shawarma (Lebanon -- Middle East)</a>, <a href=\"https://spoonacular.com/recipes/chicken-shawarma-533342\">Chicken Shawarma</a>, and <a href=\"https://spoonacular.com/recipes/chicken-shawarma-228032\">Chicken Shawarma</a> for similar recipes."},{"title":"Maple-Soy Glazed Salmon (America's Test Kitchen)","image":"https://spoonacular.com/recipeImages/86915-556x370.jpg","id":86915,"extendedIngredients":[{"id":"19911","name":"maple syrup","original":"1/2 cup maple syrup"},{"id":"15076","name":"salmon fillets","original":"4 (6 ounce) salmon fillets (1 1/4 inches thick)"},{"id":"12023","name":"sesame seeds","original":"2 teaspoons sesame seeds, toasted"},{"id":"16124","name":"soy sauce","original":"1/4 cup soy sauce"}],"summary":"Need a <b>gluten free, dairy free, fodmap friendly, and pescatarian main course</b>? Maple-Soy Glazed Salmon (America's Test Kitchen) could be an awesome recipe to try. One portion of this dish contains approximately <b>35g of protein</b>, <b>11g of fat</b>, and a total of <b>365 calories</b>. For <b>$4.94 per serving</b>, this recipe <b>covers 29%</b> of your daily requirements of vitamins and minerals. This recipe serves 4. 10 people have tried and liked this recipe. A mixture of soy sauce, salmon fillets, sesame seeds, and a handful of other ingredients are all it takes to make this recipe so flavorful. To use up the sesame seeds you could follow this main course with the <a href=\"https://spoonacular.com/recipes/sesame-banana-bread-80347\">Sesame Banana Bread</a> as a dessert. From preparation to the plate, this recipe takes around <b>25 minutes</b>. All things considered, we decided this recipe <b>deserves a spoonacular score of 95%</b>. This score is excellent. Try <a href=\"https://spoonacular.com/recipes/sweet-chili-glazed-tofu-with-bok-choy-americas-test-kitchen-92091\">Sweet Chili-Glazed Tofu With Bok Choy - America's Test Kitchen</a>, <a href=\"https://spoonacular.com/recipes/americas-test-kitchen-chocolate-chip-cookies-626421\">America's Test Kitchen Chocolate Chip Cookies</a>, and <a href=\"https://spoonacular.com/recipes/americas-test-kitchen-slow-cooker-beef-burgundy-143455\">America's Test Kitchen Slow Cooker Beef Burgundy</a> for similar recipes."},{"title":"Multigrain Loaf (By the Canadian Living Test Kitchen)","image":"https://spoonacular.com/recipeImages/142769-556x370.jpg","id":142769,"extendedIngredients":[{"id":"18375","name":"active yeast","original":"1 1/2 teaspoons active dry yeast"},{"id":"10120129","name":"bread flour","original":"4 cups multigrain whole wheat bread flour"},{"id":"19335","name":"granulated sugar","original":"1 teaspoon granulated sugar"},{"id":"19296","name":"honey","original":"2 tablespoons buckwheat honey (reg liquid honey, just as good)"},{"id":"1012047","name":"sea salt","original":"1 1/2 teaspoons fine sea salt"},{"id":"14412","name":"water","original":"1 cup warm water"}],"summary":"Multigrain Loaf (By the Canadian Living Test Kitchen) is a <b>dairy free and vegetarian</b> side dish. For <b>18 cents per serving</b>, this recipe <b>covers 5%</b> of your daily requirements of vitamins and minerals. One serving contains <b>167 calories</b>, <b>6g of protein</b>, and <b>1g of fat</b>. This recipe serves 12. 1 person has made this recipe and would make it again. From preparation to the plate, this recipe takes about <b>15 hours and 40 minutes</b>. A mixture of buckwheat honey, multigrain bread flour, granulated sugar, and a handful of other ingredients are all it takes to make this recipe so delicious. All things considered, we decided this recipe <b>deserves a spoonacular score of 42%</b>. This score is good. Similar recipes include <a href=\"https://spoonacular.com/recipes/multigrain-toasts-with-scrambled-eggs-and-canadian-bacon-186538\">Multigrain Toasts with Scrambled Eggs and Canadian Bacon</a>, <a href=\"https://spoonacular.com/recipes/americas-test-kitchen-chocolate-chip-cookies-626421\">America's Test Kitchen Chocolate Chip Cookies</a>, and <a href=\"https://spoonacular.com/recipes/maple-soy-glazed-salmon-americas-test-kitchen-86915\">Maple-Soy Glazed Salmon (America's Test Kitchen)</a>."},{"title":"Fresh vs. Canned Pumpkin: I put them to the test","image":"https://spoonacular.com/recipeImages/558628-556x370.jpg","id":558628,"extendedIngredients":[{"id":"1125","name":"egg yolks","original":"2 egg yolks"},{"id":"1123","name":"eggs","original":"2 eggs"},{"id":"1012010","name":"ground cinnamon","original":"2 tsp ground cinnamon"},{"id":"2011","name":"ground cloves","original":"1/4 tsp ground cloves"},{"id":"2021","name":"ground ginger","original":"1/4 tsp ground ginger"},{"id":"2025","name":"ground nutmeg","original":"1/4 tsp ground nutmeg"},{"id":"1053","name":"heavy cream","original":"1 cup heavy cream"},{"id":"18334","name":"pie crust","original":"1 pie crust (you can use my recipe here)"},{"id":"11426","name":"pumpkin pie mix","original":"1 small pie pumpkin, to yield 2 cups pumpkin puree"},{"id":"2047","name":"salt","original":"1/2 tsp salt"},{"id":"19335","name":"sugar","original":"1 cup sugar"}],"summary":"Fresh vs. Canned Pumpkin: I put them to the test might be just the side dish you are searching for. This recipe makes 10 servings with <b>263 calories</b>, <b>3g of protein</b>, and <b>15g of fat</b> each. For <b>48 cents per serving</b>, this recipe <b>covers 4%</b> of your daily requirements of vitamins and minerals. Head to the store and pick up ground ginger, eggs, pie crust, and a few other things to make it today. From preparation to the plate, this recipe takes roughly <b>45 minutes</b>. 655 people have tried and liked this recipe. All things considered, we decided this recipe <b>deserves a spoonacular score of 24%</b>. This score is not so amazing. Try <a href=\"https://spoonacular.com/recipes/shortcut-pumpkin-butter-with-canned-pumpkin-196697\">Shortcut Pumpkin Butter (with Canned Pumpkin)</a>, <a href=\"https://spoonacular.com/recipes/fresh-homemade-pumpkin-puree-pumpkin-bread-621597\">Fresh Homemade Pumpkin Puree & Pumpkin Bread</a>, and <a href=\"https://spoonacular.com/recipes/this-is-a-test-517956\">This Is A Test…</a> for similar recipes."},{"title":"Vanilla Cupcake – The Ultimate Vanilla Cupcake Test Baked by 50 Bakers and Counting","image":"https://spoonacular.com/recipeImages/518700-556x370.jpg","id":518700,"extendedIngredients":[{"id":"18371","name":"baking powder","original":"1 1/2 teaspoons baking powder"},{"id":"18372","name":"baking soda","original":"1/2 teaspoon baking soda"},{"id":"10020129","name":"cake flour","original":"1 3/4 cups (175 grams) cake flour, not self-rising"},{"id":"1123","name":"eggs","original":"2 large eggs, room temperature"},{"id":"1077","name":"full-fat milk","original":"1/3 cup (75 grams) full-fat sour cream"},{"id":"19335","name":"granulated sugar","original":"1 cup (225 grams) granulated sugar"},{"id":"2047","name":"salt","original":"1/2 teaspoon salt"},{"id":"1145","name":"unsalted butter","original":"1/4 cup (57 grams) unsalted butter, room temperature"},{"id":"93622","name":"vanilla bean","original":"1 vanilla bean"},{"id":"2050","name":"vanilla extract","original":"1 tablespoon pure (not imitation) vanilla extract"},{"id":"4513","name":"vegetable oil","original":"1/4 cup canola oil or vegetable oil (60 ml)"},{"id":"1077","name":"whole milk","original":"2/3 cup (160 ml) whole milk"}],"summary":"The recipe Vanilla Cupcake – The Ultimate Vanilla Cupcake Test Baked by 50 Bakers and Counting could satisfy your American craving in approximately <b>45 minutes</b>. One serving contains <b>172 calories</b>, <b>2g of protein</b>, and <b>8g of fat</b>. For <b>54 cents per serving</b>, you get a dessert that serves 16. Head to the store and pick up full-fat cream, baking soda, vanilla bean, and a few other things to make it today. 26605 people have tried and liked this recipe. All things considered, we decided this recipe <b>deserves a spoonacular score of 15%</b>. This score is not so outstanding. Try <a href=\"https://spoonacular.com/recipes/chocolate-cupcake-the-ultimate-chocolate-cupcake-test-baked-by-50-bakers-and-counting-518497\">Chocolate Cupcake – The Ultimate Chocolate Cupcake Test Baked by 50 Bakers and Counting</a>, <a href=\"https://spoonacular.com/recipes/the-go-to-vanilla-cupcake-63813\">The Go-to Vanilla Cupcake</a>, and <a href=\"https://spoonacular.com/recipes/pumpkin-chili-with-abuelita-two-ways-cupcake-and-non-cupcake-my-first-cupcake-pairing-519027\">Pumpkin Chili with Abuelita Two Ways – Cupcake and Non-Cupcake: My First Cupcake Pairing</a> for similar recipes."},{"title":"Neapolitan Fudge and a trip to the Duncan Hines Test Kitchen","image":"https://spoonacular.com/recipeImages/490710-556x370.jpg","id":490710,"extendedIngredients":[{"id":"1001","name":"butter","original":"12 Tablespoons butter, melted"},{"id":"1095","name":"condensed milk","original":"1/2 cup + 2 Tablespoons sweetened condensed milk"},{"id":"10018192","name":"cookie crumbs","original":"4 cups crushed chocolate cookie crumbs"},{"id":"19230","name":"frosting","original":"1 packet Duncan Hines Strawberry Shortcake Frosting Creation"},{"id":"93644","name":"marshmallow cream","original":"3/4 cup marshmallow cream"},{"id":"10019087","name":"white chocolate chips","original":"1 1/2 cups white chocolate chips"}],"summary":"You can never have too many dessert recipes, so give Neapolitan Fudge and a trip to the Duncan Hines Test Kitchen a try. This recipe serves 72 and costs 16 cents per serving. One serving contains <b>76 calories</b>, <b>1g of protein</b>, and <b>5g of fat</b>. This recipe is liked by 14409 foodies and cooks. If you have chocolate chips, duncan hines strawberry shortcake frosting creation, marshmallow cream, and a few other ingredients on hand, you can make it. From preparation to the plate, this recipe takes approximately <b>30 minutes</b>. All things considered, we decided this recipe <b>deserves a spoonacular score of 8%</b>. This score is very bad (but still fixable). Try <a href=\"https://spoonacular.com/recipes/duncan-hines-cake-mixes-779937\">Duncan Hines Cake Mixes</a>, <a href=\"https://spoonacular.com/recipes/maple-soy-glazed-salmon-americas-test-kitchen-86915\">Maple-Soy Glazed Salmon (America's Test Kitchen)</a>, and <a href=\"https://spoonacular.com/recipes/multigrain-loaf-by-the-canadian-living-test-kitchen-142769\">Multigrain Loaf (By the Canadian Living Test Kitchen)</a> for similar recipes."}]
    for (x in json) {
      delete json[x]['extendedIngredients'];
      delete json[x]['summary'];
    }
    this.setState({recipes: json})
    /*
    document.getElementById("recipesList").innerHTML = null;
    for (x in json) {
        let recipe = json[x];
        console.log(recipe)
      document.getElementById("recipesList").innerHTML += "<tr>"

      var rec = document.createElement("td");
      rec.setAttribute("id", "image");
      var img = document.createElement("img");
      img.setAttribute("src", recipe.image);
      img.setAttribute("width", 150);
      img.setAttribute("height", 150);
      rec.appendChild(img);
      document.getElementById("recipesList").appendChild(rec);

      var title = document.createElement("td");
      title.setAttribute("id", "title");
      title.innerHTML = recipe.title;
      document.getElementById("recipesList").appendChild(title);

      var buttontd = document.createElement("td");
      buttontd.appendChild(this.createStar())
      document.getElementById("recipesList").appendChild(buttontd);
      document.getElementById("recipesList").innerHTML += "</tr>"
    }
    */
  };

  componentDidMount() {
    this.getInfo();
  }

  render() {
    const { search } = this.state;
    /*const { dashboards } = this.state;*/
    return (
      <div>
        <svg>
          <symbol id="down" viewBox="0 0 16 16">
            <polygon points="3.81 4.38 8 8.57 12.19 4.38 13.71 5.91 8 11.62 2.29 5.91 3.81 4.38" />
          </symbol>
          <symbol id="users" viewBox="0 0 16 16">
            <path d="M8,0a8,8,0,1,0,8,8A8,8,0,0,0,8,0ZM8,15a7,7,0,0,1-5.19-2.32,2.71,2.71,0,0,1,1.7-1,13.11,13.11,0,0,0,1.29-.28,2.32,2.32,0,0,0,.94-.34,1.17,1.17,0,0,0-.27-.7h0A3.61,3.61,0,0,1,5.15,7.49,3.18,3.18,0,0,1,8,4.07a3.18,3.18,0,0,1,2.86,3.42,3.6,3.6,0,0,1-1.32,2.88h0a1.13,1.13,0,0,0-.27.69,2.68,2.68,0,0,0,.93.31,10.81,10.81,0,0,0,1.28.23,2.63,2.63,0,0,1,1.78,1A7,7,0,0,1,8,15Z" />
          </symbol>
          <symbol id="signout" viewBox="0 0 512 512">
            <path d="M497 273L329 441c-15 15-41 4.5-41-17v-96H152c-13.3 0-24-10.7-24-24v-96c0-13.3 10.7-24 24-24h136V88c0-21.4 25.9-32 41-17l168 168c9.3 9.4 9.3 24.6 0 34zM192 436v-40c0-6.6-5.4-12-12-12H96c-17.7 0-32-14.3-32-32V160c0-17.7 14.3-32 32-32h84c6.6 0 12-5.4 12-12V76c0-6.6-5.4-12-12-12H96c-53 0-96 43-96 96v192c0 53 43 96 96 96h84c6.6 0 12-5.4 12-12z"></path>
          </symbol>
          <symbol id="collection" viewBox="0 0 16 16">
            <rect width="7" height="7" />
            <rect y="9" width="7" height="7" />
            <rect x="9" width="7" height="7" />
            <rect x="9" y="9" width="7" height="7" />
          </symbol>
          <symbol id="charts" viewBox="0 0 16 16">
            <polygon points="0.64 7.38 -0.02 6.63 2.55 4.38 4.57 5.93 9.25 0.78 12.97 4.37 15.37 2.31 16.02 3.07 12.94 5.72 9.29 2.21 4.69 7.29 2.59 5.67 0.64 7.38" />
            <rect y="9" width="2" height="7" />
            <rect x="12" y="8" width="2" height="8" />
            <rect x="8" y="6" width="2" height="10" />
            <rect x="4" y="11" width="2" height="5" />
          </symbol>
          <symbol id="comments" viewBox="0 0 16 16">
            <path d="M0,16.13V2H15V13H5.24ZM1,3V14.37L5,12h9V3Z" />
            <rect x="3" y="5" width="9" height="1" />
            <rect x="3" y="7" width="7" height="1" />
            <rect x="3" y="9" width="5" height="1" />
          </symbol>
          <symbol id="pages" viewBox="0 0 16 16">
            <rect
              x="4"
              width="12"
              height="12"
              transform="translate(20 12) rotate(-180)"
            />
            <polygon points="2 14 2 2 0 2 0 14 0 16 2 16 14 16 14 14 2 14" />
          </symbol>
          <symbol id="appearance" viewBox="0 0 16 16">
            <path d="M3,0V7A2,2,0,0,0,5,9H6v5a2,2,0,0,0,4,0V9h1a2,2,0,0,0,2-2V0Zm9,7a1,1,0,0,1-1,1H9v6a1,1,0,0,1-2,0V8H5A1,1,0,0,1,4,7V6h8ZM4,5V1H6V4H7V1H9V4h1V1h2V5Z" />
          </symbol>
          <symbol id="trends" viewBox="0 0 16 16">
            <polygon points="0.64 11.85 -0.02 11.1 2.55 8.85 4.57 10.4 9.25 5.25 12.97 8.84 15.37 6.79 16.02 7.54 12.94 10.2 9.29 6.68 4.69 11.76 2.59 10.14 0.64 11.85" />
          </symbol>
          <symbol id="settings" viewBox="0 0 16 16">
            <rect x="9.78" y="5.34" width="1" height="7.97" />
            <polygon points="7.79 6.07 10.28 1.75 12.77 6.07 7.79 6.07" />
            <rect x="4.16" y="1.75" width="1" height="7.97" />
            <polygon points="7.15 8.99 4.66 13.31 2.16 8.99 7.15 8.99" />
            <rect x="1.28" width="1" height="4.97" />
            <polygon points="3.28 4.53 1.78 7.13 0.28 4.53 3.28 4.53" />
            <rect x="12.84" y="11.03" width="1" height="4.97" />
            <polygon points="11.85 11.47 13.34 8.88 14.84 11.47 11.85 11.47" />
          </symbol>
          <symbol id="options" viewBox="0 0 16 16">
            <path d="M8,11a3,3,0,1,1,3-3A3,3,0,0,1,8,11ZM8,6a2,2,0,1,0,2,2A2,2,0,0,0,8,6Z" />
            <path d="M8.5,16h-1A1.5,1.5,0,0,1,6,14.5v-.85a5.91,5.91,0,0,1-.58-.24l-.6.6A1.54,1.54,0,0,1,2.7,14L2,13.3a1.5,1.5,0,0,1,0-2.12l.6-.6A5.91,5.91,0,0,1,2.35,10H1.5A1.5,1.5,0,0,1,0,8.5v-1A1.5,1.5,0,0,1,1.5,6h.85a5.91,5.91,0,0,1,.24-.58L2,4.82A1.5,1.5,0,0,1,2,2.7L2.7,2A1.54,1.54,0,0,1,4.82,2l.6.6A5.91,5.91,0,0,1,6,2.35V1.5A1.5,1.5,0,0,1,7.5,0h1A1.5,1.5,0,0,1,10,1.5v.85a5.91,5.91,0,0,1,.58.24l.6-.6A1.54,1.54,0,0,1,13.3,2L14,2.7a1.5,1.5,0,0,1,0,2.12l-.6.6a5.91,5.91,0,0,1,.24.58h.85A1.5,1.5,0,0,1,16,7.5v1A1.5,1.5,0,0,1,14.5,10h-.85a5.91,5.91,0,0,1-.24.58l.6.6a1.5,1.5,0,0,1,0,2.12L13.3,14a1.54,1.54,0,0,1-2.12,0l-.6-.6a5.91,5.91,0,0,1-.58.24v.85A1.5,1.5,0,0,1,8.5,16ZM5.23,12.18l.33.18a4.94,4.94,0,0,0,1.07.44l.36.1V14.5a.5.5,0,0,0,.5.5h1a.5.5,0,0,0,.5-.5V12.91l.36-.1a4.94,4.94,0,0,0,1.07-.44l.33-.18,1.12,1.12a.51.51,0,0,0,.71,0l.71-.71a.5.5,0,0,0,0-.71l-1.12-1.12.18-.33a4.94,4.94,0,0,0,.44-1.07l.1-.36H14.5a.5.5,0,0,0,.5-.5v-1a.5.5,0,0,0-.5-.5H12.91l-.1-.36a4.94,4.94,0,0,0-.44-1.07l-.18-.33L13.3,4.11a.5.5,0,0,0,0-.71L12.6,2.7a.51.51,0,0,0-.71,0L10.77,3.82l-.33-.18a4.94,4.94,0,0,0-1.07-.44L9,3.09V1.5A.5.5,0,0,0,8.5,1h-1a.5.5,0,0,0-.5.5V3.09l-.36.1a4.94,4.94,0,0,0-1.07.44l-.33.18L4.11,2.7a.51.51,0,0,0-.71,0L2.7,3.4a.5.5,0,0,0,0,.71L3.82,5.23l-.18.33a4.94,4.94,0,0,0-.44,1.07L3.09,7H1.5a.5.5,0,0,0-.5.5v1a.5.5,0,0,0,.5.5H3.09l.1.36a4.94,4.94,0,0,0,.44,1.07l.18.33L2.7,11.89a.5.5,0,0,0,0,.71l.71.71a.51.51,0,0,0,.71,0Z" />
          </symbol>
          <symbol id="collapse" viewBox="0 0 16 16">
            <polygon points="11.62 3.81 7.43 8 11.62 12.19 10.09 13.71 4.38 8 10.09 2.29 11.62 3.81" />
          </symbol>
          <symbol id="search" viewBox="0 0 16 16">
            <path d="M6.57,1A5.57,5.57,0,1,1,1,6.57,5.57,5.57,0,0,1,6.57,1m0-1a6.57,6.57,0,1,0,6.57,6.57A6.57,6.57,0,0,0,6.57,0Z" />
            <rect
              x="11.84"
              y="9.87"
              width="2"
              height="5.93"
              transform="translate(-5.32 12.84) rotate(-45)"
            />
          </symbol>
        </svg>
        <header className="page-header">
          <nav>
            <Link to="/dashboard">
              <img
                className="logo"
                src="https://cdn.discordapp.com/attachments/443699822025900033/703629640773795850/fork.svg"
                alt="forecastr logo"
              />
            </Link>
            <button
              className="toggle-mob-menu"
              aria-expanded="false"
              aria-label="open menu"
            >
              <svg width="20" height="20" aria-hidden="true">
                <use href="#down"></use>
              </svg>
            </button>
            <ul className="admin-menu">
              <li className="menu-heading">
                <h3>Inventories</h3>
              </li>
              <li>
                <a href="/dashboard">
                  <svg>
                    <use href="#collection"></use>
                  </svg>
                  <span>Inventories</span>
                </a>
              </li>
              <li>
                <a href="/dashboard/wishlists">
                  <svg>
                    <use href="#collection"></use>
                  </svg>
                  <span>Wishlists</span>
                </a>
              </li>
              <li>
                <a href="/dashboard/shoppinglists">
                  <svg>
                    <use href="#collection"></use>
                  </svg>
                  <span>Shopping Lists</span>
                </a>
              </li>
              <li>
                <a href="/dashboard/recipes">
                  <svg>
                    <use href="#collection"></use>
                  </svg>
                  <span>Recipes</span>
                </a>
              </li>
              <li className="menu-heading">
                <h3>Settings</h3>
              </li>
              <li>
                <a href="/dashboard/userinfo">
                  <svg>
                    <use href="#users"></use>
                  </svg>
                  <span>Account</span>
                </a>
              </li>
              <li>
                <a href="/dashboard/friends">
                  <svg>
                    <use href="#users"></use>
                  </svg>
                  <span>Friends</span>
                </a>
              </li>
              <li>
                <Link to="/" onClick={this.removeToken}>
                  <svg>
                    <use href="#signout"></use>
                  </svg>
                  <span>Logout</span>
                </Link>
              </li>
              <li>
                <button
                  className="collapse-btn"
                  aria-expanded="true"
                  aria-label="collapse menu"
                  onClick={this.collapseBar}
                >
                  <svg aria-hidden="true">
                    <use href="#collapse"></use>
                  </svg>
                  <span>Collapse</span>
                </button>
              </li>
            </ul>
          </nav>
        </header>
        <section className="page-content">
          <section className="search-and-user">
            <form onSubmit={this.submitHandler}>
              <input
                type="text"
                placeholder="Search recipes..."
                name="search"
                value={search}
                onChange={this.changeHandler}
              />
              <button type="submit">
                <svg aria-hidden="true">
                  <use href="#search"></use>
                </svg>
              </button>
            </form>
            <div className="admin-profile">
              <span className="greeting">Hello {this.state.name}</span>
              <div className="notifications">
                <svg>
                  <use href="#users"></use>
                </svg>
              </div>
            </div>
          </section>
          <section className="grid">
            <article className="inventories">
              <div className="inventories-text">Recipes</div>
              <RecipesTable data={this.state.recipes}/>
            </article>
          </section>
          <footer className="page-footer">
            <small>
              Made with <span>❤</span> by{" "}
              <a href="http://www.uminho.pt/">Grupo 1</a>
            </small>
          </footer>
        </section>
      </div>
    );
  }
}
