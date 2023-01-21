<template>
  <b-dropdown-item v-on:click="redirect"> {{field}}</b-dropdown-item>
</template>

<script>
export default {
  name: 'MagazineField',
  props: {
    field: String,
    id: Number,
  },
  methods : {
    redirect() {
      this.$router.push({path: '/field/' + this.id})
    }
  }
}
</script>