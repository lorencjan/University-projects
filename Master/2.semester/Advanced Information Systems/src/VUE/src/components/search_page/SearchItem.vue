<template>
  <div>
    <b-row>
      <b-col v-if="type !== 'genre' && type !== 'field'" cols="2">
          <b-img class="tile-img" :src="img" alt="Image" v-if="img !== null"></b-img>
          <font-awesome-icon icon="fa-solid fa-book" size="5x" class="my-3  img-cover" v-else/>
      </b-col>

      <b-col v-if="type === 'book' || type ==='magazine'" cols="5"> <!-- book/magazine -->
        <router-link :to="{path: root + id}">
          <h5 class="text-left mt-2">
            {{ item.name }}
          </h5>
        </router-link>
        <p align="left" class="mb-0">
          <small v-for="(author,index) in item.authors"
                 :key="index">
            {{author.name}} {{author.surname}}</small>
        </p>
        <div align="left">
          <b-badge pill variant="warning">{{ item.language }}</b-badge>
        </div>
        <div align="left" v-if="item.electronicCopyExemplars.length > 0">
          <b-badge pill variant="success">Available electronic copies</b-badge>
        </div>
        <div align="left" v-if="item.hardCopyExemplars.length > 0">
          <b-badge v-if="item.hardCopyExemplars.length === 1" pill variant="info">{{item.hardCopyExemplars.length}} hardcopy</b-badge>
          <b-badge v-else pill variant="info">{{item.hardCopyExemplars.length}} hardcopies</b-badge>

        </div>

      </b-col>
      <b-col v-if="type === 'author'" cols="5"> <!-- author -->
          <router-link :to="{path: root + id}">
            <h5 class="text-left mt-2">
              {{ item.name }} {{item.surname}}
            </h5>
          </router-link>
        <div align="left">
          <b-badge v-if="item.books.length > 1" pill variant="success">{{item.books.length}} books</b-badge>
          <b-badge v-if="item.books.length === 1" pill variant="success">{{item.books.length}} book</b-badge>
          </div>
        <div align="left">
          <b-badge v-if="item.magazines.length > 1" pill variant="info">{{item.magazines.length}} magazines</b-badge>
          <b-badge v-if="item.magazines.length === 1" pill variant="info">{{item.magazines.length}} magazines</b-badge>
        </div>
        </b-col>
      <b-col v-if="type === 'genre' || type === 'field'" cols="5"> <!-- genre/field -->
        <router-link :to="{path: root + id}">
          <h5 class="text-left mt-2">
            {{ item.name }}
          </h5>
        </router-link>
      </b-col>
    </b-row>
  </div>
</template>
<script>
export default {
  name: 'SearchItem',
  props: {
    item: {},
    type: String,
    id: Number,
    img: String,
    root: String
  },
  computed: {
    imageSrc: function (){
      const base64String = btoa(String.fromCharCode(...new Uint8Array(this.img)));
      return "data:image/png;base64," + base64String;
    }
  }
}
</script>

<style scoped>
.img-cover{
  color:#24433e;
}
</style>