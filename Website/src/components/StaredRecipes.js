import React from "react";
import Swal from "sweetalert2";
import axios from "axios";
import RecipesTable from "./RecipesTable";

export default class InventoryList extends React.Component {

  render() {
    return (
      <section className="grid">
        <article className="inventories">
        </article>
      </section>
    );
  }
}
