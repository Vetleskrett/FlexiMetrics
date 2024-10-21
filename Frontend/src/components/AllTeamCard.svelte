<script lang="ts">
	import type { Team } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import { Progress } from '$lib/components/ui/progress/index.js';

	export let teams: Team[], courseId: string;
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Content class="p-0">
		<div class="flex flex-col">
			<div class="flex items-center px-6 text-sm font-bold">
				<h1 class="w-1/2">Team</h1>
				<h1 class="w-1/2">Members</h1>
				<h1 class="w-1/2 pl-5">Progress</h1>
			</div>
			{#each teams as team}
				<Separator class="w-full" />
				<a href="/courses/{courseId}/teams/{team.id}" class="hover:bg-blue-50">
					<div class="flex items-center px-6 py-4">
						<h1 class="w-1/2">{team.id}</h1>
						<div class="w-1/2">
							{#each team.students as student}
								<h1>{student.email}</h1>
								<Separator class="w-full" />
							{/each}
						</div>
						<div class="w-1/2 pl-5">
							<Progress value={team.complete} />
						</div>
					</div>
				</a>
			{/each}
		</div>
	</Card.Content>
</Card.Root>
