import React from "react";
import axios from "axios";
import Swal from "sweetalert2";

export default class RecipesTable extends React.Component {
  getKeys = function () {
    if (this.props.data.length) return Object.keys(this.props.data[0]);
    else return [];
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
      return (
        <RenderRow
          key={index}
          data={row}
          keys={keys}
          stared={this.props.stared}
          handler={this.props.handler}
        />
      );
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
  function createButton() {
    ;
    if (props.stared) {
      return (
        <td
          id="fav-button"
          onClick={() => {
            unstarRecipe(props.data["id"]);
          }}
        >
          <span
            className="edit-button"
            role="img"
            aria-label="jsx-a11y/aria-proptypes"
            title="Unstar"
          >
            ⭐
          </span>
        </td>
      );
    } else {
      return (
        <td
          id="fav-button"
          onClick={() => {
            starRecipe(props.data["id"]);
          }}
        >
          <span
            className="edit-button"
            role="img"
            aria-label="jsx-a11y/aria-proptypes"
            title="Star"
          >
            ⭐
          </span>
        </td>
      );
    }
  }

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
        Swal.fire("Recipe Stared!", "This recipe has been stared", "success");
      })
      .catch((error) => {
        console.log(error);
      });
  }

  function unstarRecipe(e) {
    let token = localStorage.getItem("auth");
    const form = new FormData();
    form.append("id", e);

    axios
      .post("http://localhost:1331/recipe/unstar", form, {
        headers: { "Content-Type": "multipart/form-data", auth: token },
        withCredentials: true,
      })
      .then((response) => {
        const token = response.headers["auth"];
        localStorage.setItem("auth", token);
        Swal.fire(
          "Recipe Unstared!",
          "This recipe has been unstared",
          "success"
        );
        props.handler();
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
        <td id="title">
          <a href={"/dashboard/recipe/get/" + props.data["id"]}>
            {props.data["title"]}
          </a>
        </td>
        {createButton()}
      </tr>
    );
  } else {
    return null;
  }
};
