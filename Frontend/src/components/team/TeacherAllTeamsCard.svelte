<script lang="ts">
	import type { SlimProgress, Team } from 'src/types/';
	import * as Card from '$lib/components/ui/card';
	import * as Table from '$lib/components/ui/table';
	import { CircleCheck, CircleX } from 'lucide-svelte';

	export let teams: Team[];
	export let teamsProgress: SlimProgress[];
	export let courseId: string;
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Content class="p-0">
		<Table.Root class="h-full">
			<Table.Header>
				<Table.Row>
					<Table.Head class="flex h-8 items-center justify-center px-6 font-bold text-black">
						Team
					</Table.Head>
					<Table.Head class="h-8 px-6 font-bold text-black">Members</Table.Head>
					<Table.Head class="h-8 px-6 font-bold text-black">Assignments Delivered</Table.Head>
				</Table.Row>
			</Table.Header>
			<Table.Body>
				{#each teams as team}
					{@const teamProgress = teamsProgress.find((x) => x.id == team.id)}

					<a href="/teacher/courses/{courseId}/teams/{team.id}" class="table-row hover:bg-blue-50">
						<Table.Cell class="flex h-full items-center justify-center px-6">
							<p>{team.teamNr}</p>
						</Table.Cell>
						<Table.Cell class="px-6">
							{#each team.students as student}
								<p class="whitespace-nowrap">• {student.name || student.email}</p>
							{/each}
						</Table.Cell>
						<Table.Cell class="px-6">
							<div class="flex flex-row items-center gap-4">
								{#if teamProgress}
									{#each teamProgress.assignmentsProgress as assignmentProgress}
										{#if assignmentProgress.isDelivered}
											<CircleCheck color="green" />
										{:else}
											<CircleX color="red" />
										{/if}
									{/each}
								{/if}
							</div>
						</Table.Cell>
					</a>
				{/each}
			</Table.Body>
		</Table.Root>
	</Card.Content>
</Card.Root>
