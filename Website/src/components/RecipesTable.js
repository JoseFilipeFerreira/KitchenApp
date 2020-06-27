import React from "react";
import axios from "axios";

export default class RecipesTable extends React.Component {
  constructor(props) {
    super(props);
    this.getHeader = this.getHeader.bind(this);
    this.getRowsData = this.getRowsData.bind(this);
    this.getKeys = this.getKeys.bind(this);
  }

  getKeys = function () {
    return Object.keys(this.props.data[0]);
  };

  getHeader = function () {
    var keys = this.getKeys();
    if (keys.length) {
      return (
        <tr>
          <th>Image</th>
          <th>Title</th>
          <th></th>
        </tr>
      );
    } else {
      return null;
    }
  };

  getRowsData = function () {
    var items = this.props.data;
    var keys = this.getKeys();
    delete keys[2];
    return items.map((row, index) => {
      return <RenderRow key={index} data={row} keys={keys} />;
    });
  };

  render() {
    return (
      <div>
        <table id="recipesList">
          <thead>{this.getHeader()}</thead>
          <tbody>{this.getRowsData()}</tbody>
        </table>
      </div>
    );
  }
}
const RenderRow = (props) => {
  function starRecipe(e) {
    let token = localStorage.getItem("auth");
    const form = new FormData();
    form.append("id", e);

    axios
      .post("http://localhost:1331/recipe/star", form, {
        headers: { "Content-Type": "multipart/form-data", auth: token },
        withCredentials: true,
      })
      .then((response) => {
        const token = response.headers["auth"];
        localStorage.setItem("auth", token);
        alert('Recipe stared')
      })
      .catch((error) => {
        console.log(error);
      });
  }

  if (props.keys.length) {
    return (
      <tr>
        <td id="image">
          <img
            src={props.data["image"]}
            alt={props.data["title"]}
            width="150"
            heigh="150"
          ></img>
        </td>
        <td id="title">{props.data["title"]}</td>
        <td
          id="fav-button"
          onClick={() => {
            starRecipe(props.data["id"]);
          }}
        >
          ⭐
        </td>
      </tr>
    );
  } else {
    return null;
  }
};
