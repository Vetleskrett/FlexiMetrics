<script lang="ts">
    import { page } from "$app/stores";
    console.log($page.data.session)
</script>

<h1>Welcome to SvelteKit {$page.data.session?.user?.sub}</h1>
<p>Visit <a href="https://kit.svelte.dev">kit.svelte.dev</a> to read the documentation</p>
