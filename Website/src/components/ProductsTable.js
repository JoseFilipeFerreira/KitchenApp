import React from "react";
import axios from "axios";
import Swal from "sweetalert2";

export default class RecipesTable extends React.Component {

  getKeys = function () {
    if (this.props.data.length)
      return Object.keys(this.props.data[0]);
    else
      return [];
  };

  getHeader = function () {
    var keys = this.getKeys();
    if (keys.length) {
      return (
        <tr>
          <th>Name</th>
          <th>Category</th>
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
    console.log(keys)
    //delete keys[2];
    return items.map((row, index) => {
      return <RenderRow 
      key={index} 
      data={row} 
      keys={keys} 
      inventories={this.props.inventories}
      wishlists={this.props.wishlists}
      shoppinglists={this.props.shoppinglists}
      />;
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


  async function addProduct(e) {
    let lists = ['Inventory','Wishlist','Shoppinglist']

    const { value: list } = await Swal.fire({
      title: "Select list",
      input: "select",
      inputOptions: lists,
      inputPlaceholder: "Select a list",
      showCancelButton: true,
      inputValidator: (value) => {
        return new Promise((resolve) => {
          if (value) {
            resolve();
          } else {
            resolve("You need to select one");
          }
        });
      },
    });

    const choice = lists[list]

    console.log(choice)

    let names;

    switch (choice) {
      case "Inventory":
        names = props.inventories
        break
      case "Wishlist":
        names = props.wishlists
        break
      default:
        names = props.shoppinglists
        break
    }

    if (list) {
      const { value: value } = await Swal.fire({
        title: "Select " + choice,
        input: "select",
        inputOptions: Object.keys(names),
        inputPlaceholder: "Select a " + choice,
        showCancelButton: true,
        inputValidator: (value) => {
          return new Promise((resolve) => {
            if (value) {
              resolve();
            } else {
              resolve("You need to select one");
            }
          });
        },
      });
    }
  }




  if (props.keys.length) {
    return (
      <tr>
        <td id="title">{props.data["_name"]}</td>
        <td id="title">{props.data["_category"]}</td>
        <td
        id="fav-button"
        onClick={() => {
          addProduct(props.data["_guid"]);
        }}
      >
        <span title="Add Product">➕</span>
        
      </td>
      </tr>
    );
  } else {
    return null;
  }
};
