<script lang="ts">
	import type { Student } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import { Progress } from '$lib/components/ui/progress/index.js';

	export let students: Student[];
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Content class="p-0">
		<div class="flex flex-col">
			<div class="flex items-center px-6 text-sm font-bold">
				<h1 class="w-1/2">Name</h1>
				<h1 class="w-1/2">Email</h1>
				<h1 class="w-1/2 pl-5">Team</h1>
			</div>
			{#each students as student}
				<Separator class="w-full" />
                <div class="flex items-center px-6 py-4">
                    <h1 class="w-1/2">{student.name}</h1>
                    <h1 class="w-1/2">{student.email}</h1>
                    <h1 class="w-1/2 pl-5">{0}</h1>
                </div>
			{/each}
		</div>
	</Card.Content>
</Card.Root>
