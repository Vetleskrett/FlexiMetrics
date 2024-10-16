<script lang="ts">
	import { Button } from '$lib/components/ui/button';

	export let color: 'green' | 'blue' | 'yellow',
		href: string = '';
</script>

<Button
	{href}
	size="sm"
	class="bg-button-{color} hover:bg-button-{color}-hover flex flex-row justify-between gap-2 px-3"
>
	<slot />
</Button>
