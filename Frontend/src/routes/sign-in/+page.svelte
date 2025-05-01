<script lang="ts">
	import { page } from '$app/stores';
	import { signIn } from '@auth/sveltekit/client';
	import { onMount } from 'svelte';

    const callbackUrl = $page.url.searchParams.get('callbackUrl') ?? '/';

    onMount(async () => {
        await signIn('feide', {
            callbackUrl: callbackUrl
        })
    });
</script>
