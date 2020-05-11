<template>
  <q-layout view="hHh lpR fFf">
    <q-page-container>
      <q-page class="flex flex-center column">
        <form @submit="submit">
          <q-card style="min-width:200px">
            <q-card-section><div class="text-h6">Login</div></q-card-section>
            <q-separator />
            <q-card-section>
              <q-input
                label="User name"
                v-model="username"
              />
              <q-input
                label="Password"
                v-model="password"
                type="password"
              />
            </q-card-section>
            <q-separator />
            <q-card-actions class="justify-end">
              <q-btn 
                color="primary"
                label="Login"
                type="submit"
              />
            </q-card-actions>
          </q-card>
        </form>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script>

export default {
  name: 'LoginPage',
  data () {
    return  {
      username: null,
      password: null
    };
  },
  methods: {
    submit (evt) {
      evt.preventDefault();

      this.$auth.login(this.username, this.password);
      this.$router.replace({ name: 'index' });
    }
  }
}
</script>
