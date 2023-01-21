<template>
  <div>
    <b-container class="bv-example-row">
      <b-row>
        <b-col><h1>{{ name }}</h1></b-col>
        <b-col><router-link :to="fullPage">
          <h3>More ></h3>
        </router-link></b-col>
      </b-row>

      <b-row>
        <AuthorTile v-for="load in data.slice(0,5)"
                    :img="load.photographPath"
                    :name = load.name
                    :surname = load.surname
                    :id = "load.id"
                    :dateOfBirth = "new Date(load.dateOfBirth)"
        >
        </AuthorTile>
      </b-row>
    </b-container>
  </div>
</template>


<script>
import AuthorTile from "@/components/genre_page/AuthorTile";

export default {
  name: 'AuthorSection',
  components: {
    AuthorTile,
  },
  props: {
    name: String,
    data: {},
    fullPage: String,
  },

}

</script>
