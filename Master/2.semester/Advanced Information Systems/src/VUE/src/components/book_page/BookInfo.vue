<template>
  <div id="book-info" class="text-left px-4 py-2">
    <b-row>
      <b-col>
        <h1 id="headline" class="display-4 mb-1">{{ title }}</h1>
      </b-col>
    </b-row>

    <b-row class="text-left">
      <b-col>
        <h2>
          By
          <span v-for="author in authors">
            <router-link :to="{path: '/authors/'+author.id}" class="author-link">{{author.name}} {{author.surname}}</router-link>&nbsp;
          </span>
        </h2>
      </b-col>
    </b-row>
    <b-row class="text-right">
      <b-col v-if="genres !== undefined">
        <span v-for="genre in genres">
          <router-link :to="{path: '/genre/'+genre.id}" class="author-link">{{genre.name}}</router-link>&nbsp;
        </span>
      </b-col>
      <b-col v-if="fields !== undefined">
        <span v-for="field in fields">
          <router-link :to="{path: '/field/'+field.id}" class="author-link">{{field.name}}</router-link>&nbsp;
        </span>
      </b-col>
    </b-row>

    <b-row >
      <b-col class="text-justify">
        <h5>Description: </h5>
        {{ description }}
      </b-col>
    </b-row>

    <b-row class="mt-4">
      <b-col class="text-left">
        <small>Edition {{ publicationNumber }} </small>
      </b-col>
      <b-col class="text-right">
        <small v-if="isbn !== undefined" >ISBN {{ isbn }} </small>
        <small v-if="!isNaN(issn)" >ISSN {{ issn }} </small>
      </b-col>
    </b-row>

  </div>
</template>

<script>
export default {
  name: "BookInfo",
  props: {
    title: String,
    publicationNumber: String,
    authors: {},
    isbn: String,
    issn: Number,
    genres: [],
    fields: [],
    description: String,
  }
}
</script>

<style scoped>
#book-info{
  background-color: #24433e;
  color: #d0d3d3 !important;
}
h5{
  color: #d0d3d3 !important;
}
#headline{
  line-height: 0.95;
  color: #d0d3d3 !important;
}
small{
  color: #cbcbcb !important;
}
.author-link{
  color: inherit;
  text-decoration: underline;
}
.author-link:hover{
  color: #6a6a6a;
}

</style>