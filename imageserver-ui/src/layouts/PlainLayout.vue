<template>
  <q-layout view="hHh lpR fFf">
    <q-page-container>
      <form class="flex flex-center" @submit="submit">
        <div class="row">
          <q-field
            label="Username"
            v-model="username"
          />
        </div>
        <div class="row">
          <q-field
            label="Password"
            v-model="password"
            type="password"
          />
        </div>
        <div class="row">
          <q-btn 
            label="Login"
            type="submit"
          />
        </div>
      </div>
    </q-page-container>
  </q-layout>
</template>

<script>

export default {
  name: 'PlainLayout',
  data () {
    return  {
      username: null,
      password: null
    };
  },
  methods: {
    submit () {
      this.$sote.commit('app/basicAuth', { username: this.usernmae, password: this.password });
    }
  }
}
</script>
