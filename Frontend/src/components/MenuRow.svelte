<script lang="ts">
	export let href: string;
</script>

<a {href} class="flex w-full items-center justify-start gap-2 py-3 pl-6 text-lg hover:bg-blue-50">
	<slot />
</a>
