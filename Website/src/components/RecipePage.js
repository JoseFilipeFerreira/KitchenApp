import React from "react";

export default class RecipePage extends React.Component {
  getIngredients = function () {
    let ingredients = this.props.ingredients;
    console.log(ingredients)
    return ingredients.map((ingredient, index) => {
    return <div>{ingredient.original}</div>;
    });
  };

  render() {
    return (
      <div>
        <div id="recipeInfo">
          <img
            id="recipeImage"
            src={this.props.img}
            alt={this.props.title}
            width="300"
            heigh="300"
          />
          <div
            id="recipeSummary"
            dangerouslySetInnerHTML={{ __html: this.props.summary }}
          ></div>
          <div id="recipeIngredients"><b>Ingredients:</b> {this.getIngredients()}</div>
          <div id="recipeIngredients" dangerouslySetInnerHTML={{ __html: this.props.instructions }}></div>
        </div>
      </div>
    );
  }
}
